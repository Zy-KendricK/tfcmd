using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using AppCore;
using AppCore.Entities;
using AppCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Admin.Controllers;

[Authorize]
[RequirePermission("users.view")]
public class MembersController : BaseAdminController
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserGroupService _groupService;
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<MembersController> _logger;

    public MembersController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IUserGroupService groupService,
        IUserService userService,
        IPermissionService permissionService,
        IConfiguration configuration,
        ILogger<MembersController> logger) : base(permissionService)
    {
        _context = context;
        _userManager = userManager;
        _groupService = groupService;
        _userService = userService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string? search, int? group, string? tab, int page = 1, int pageSize = 10, bool ajax = false)
    {
        var query = _context.Users
            .Where(u => !u.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            search = search.ToLower();
            query = query.Where(u =>
                u.Email!.ToLower().Contains(search) ||
                u.UserName!.ToLower().Contains(search) ||
                (u.DisplayName != null && u.DisplayName.ToLower().Contains(search)) ||
                (u.FirstName != null && u.FirstName.ToLower().Contains(search)) ||
                (u.LastName != null && u.LastName.ToLower().Contains(search)));
        }

        if (group.HasValue)
        {
            query = query.Where(u => u.GroupMemberships.Any(m => m.UserGroupId == group.Value && m.IsActive && !m.IsDeleted));
        }

        var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        query = tab switch
        {
            "active" => query.Where(u => u.IsActive),
            "inactive" => query.Where(u => !u.IsActive),
            "new" => query.Where(u => u.CreatedAt >= monthStart),
            "online" => query.Where(u => u.LastLoginAt >= DateTime.UtcNow.AddMinutes(-15)),
            _ => query
        };

        var totalCount = await query.CountAsync();
        var users = await query
            .Include(u => u.GroupMemberships)
                .ThenInclude(m => m.UserGroup)
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Sidebar stats
        ViewBag.TotalMembers = await _context.Users.CountAsync(u => !u.IsDeleted);
        ViewBag.ActiveMembers = await _context.Users.CountAsync(u => !u.IsDeleted && u.IsActive);
        ViewBag.NewThisMonth = await _context.Users.CountAsync(u => !u.IsDeleted && u.CreatedAt >= monthStart);
        ViewBag.OnlineNow = await _context.Users.CountAsync(u => !u.IsDeleted && u.LastLoginAt >= DateTime.UtcNow.AddMinutes(-15));
        ViewBag.Groups = await _groupService.GetAllGroupsAsync();
        ViewBag.GroupId = group;
        ViewBag.Tab = tab;

        var viewModel = new MembersListViewModel
        {
            Users = users,
            Search = search,
            GroupId = group,
            Tab = tab,
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };

        Response.Headers["X-Has-More"] = (page < viewModel.TotalPages).ToString().ToLowerInvariant();
        if (ajax)
        {
            return PartialView("_ListItems", viewModel);
        }

        return View(viewModel);
    }

    public async Task<IActionResult> Details(string id)
    {
        var user = await _context.Users
            .Include(u => u.GroupMemberships)
                .ThenInclude(m => m.UserGroup)
            .Include(u => u.Posts)
            .Include(u => u.Activities)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            return NotFound();

        return View(user);
    }

    [RequirePermission("users.create")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Groups = await _groupService.GetAllGroupsAsync();
        return View(new CreateMemberViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("users.create")]
    public async Task<IActionResult> Create(CreateMemberViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Groups = await _groupService.GetAllGroupsAsync();
            return View(model);
        }

        var temporaryPassword = GenerateTemporaryPassword();
        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            DisplayName = string.IsNullOrWhiteSpace($"{model.FirstName} {model.LastName}".Trim()) ? model.Email : $"{model.FirstName} {model.LastName}".Trim(),
            Bio = model.Bio,
            IsActive = true,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, temporaryPassword);

        if (result.Succeeded)
        {
            await _userManager.AddClaimAsync(user, new Claim("RequirePasswordChange", "true"));

            // Assign the user's primary group (one group only)
            if (model.GroupId.HasValue)
            {
                await _groupService.AddUserToGroupAsync(user.Id, model.GroupId.Value, true);
            }

            var emailSent = await TrySendWelcomeEmailAsync(model.Email, temporaryPassword);
            TempData["Success"] = emailSent
                ? "Member created successfully. A temporary password has been sent to the email address."
                : "Member created successfully. A temporary password was generated for the new member.";
            if (!emailSent)
            {
                TempData["Info"] = "The temporary password could not be sent automatically. Please configure SMTP settings for email delivery.";
            }

            return RedirectToAction(nameof(Index));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        ViewBag.Groups = await _groupService.GetAllGroupsAsync();
        return View(model);
    }

    [RequirePermission("users.edit")]
    public async Task<IActionResult> Edit(string id)
    {
        var user = await _context.Users
            .Include(u => u.GroupMemberships)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            return NotFound();

        var model = new EditMemberViewModel
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            DisplayName = user.DisplayName,
            Bio = user.Bio,
            IsActive = user.IsActive,
            GroupId = user.GroupMemberships
                .Where(m => m.IsActive && !m.IsDeleted)
                .Select(m => m.UserGroupId)
                .FirstOrDefault()
        };

        ViewBag.Groups = await _groupService.GetAllGroupsAsync();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("users.edit")]
    public async Task<IActionResult> Edit(EditMemberViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Groups = await _groupService.GetAllGroupsAsync();
            return View(model);
        }

        var user = await _context.Users
            .Include(u => u.GroupMemberships)
            .FirstOrDefaultAsync(u => u.Id == model.Id);

        if (user == null)
            return NotFound();

        // Don't allow editing super admin's status if current user is not super admin
        if (user.IsSuperAdmin && !await IsSuperAdminAsync())
        {
            TempData["Error"] = "You cannot edit the super admin account.";
            return RedirectToAction(nameof(Index));
        }

        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.DisplayName = model.DisplayName ?? $"{model.FirstName} {model.LastName}";
        user.Bio = model.Bio;
        user.IsActive = model.IsActive;
        user.UpdatedAt = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);

        // Update the user's single primary group
        var currentGroupIds = user.GroupMemberships
            .Where(m => m.IsActive && !m.IsDeleted)
            .Select(m => m.UserGroupId)
            .ToList();

        foreach (var groupId in currentGroupIds.Where(g => g != model.GroupId))
        {
            await _groupService.RemoveUserFromGroupAsync(user.Id, groupId);
        }

        if (model.GroupId.HasValue)
        {
            await _groupService.AddUserToGroupAsync(user.Id, model.GroupId.Value, true);
        }

        TempData["Success"] = "Member updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("users.delete")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            await _userService.DeleteUserAsync(id);
            TempData["Success"] = "Member deleted successfully.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("users.edit")]
    public async Task<IActionResult> ToggleActive(string id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound();

        try
        {
            await _userService.SetUserActiveAsync(id, !user.IsActive);
            TempData["Success"] = user.IsActive ? "Member deactivated." : "Member activated.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    private static string GenerateTemporaryPassword()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789!@#$%^&*";
        var random = new Random();
        return string.Concat(Enumerable.Repeat(0, 12).Select(_ => chars[random.Next(chars.Length)]));
    }

    private async Task<bool> TrySendWelcomeEmailAsync(string email, string password)
    {
        var host = _configuration["Smtp:Host"];
        if (string.IsNullOrWhiteSpace(host))
        {
            _logger.LogWarning("SMTP host is not configured. Skipping welcome email for {Email}.", email);
            return false;
        }

        var fromAddress = _configuration["Smtp:FromAddress"] ?? "noreply@tfcmd.local";
        var port = int.TryParse(_configuration["Smtp:Port"], out var parsedPort) ? parsedPort : 587;
        var enableSsl = bool.TryParse(_configuration["Smtp:EnableSsl"], out var parsedSsl) && parsedSsl;
        var userName = _configuration["Smtp:UserName"];
        var passwordValue = _configuration["Smtp:Password"];
        var clubName = _configuration["Club:Name"] ?? "Tema Friends Club of Maryland";
        var websiteUrl = _configuration["Club:WebsiteUrl"] ?? $"{Request.Scheme}://{Request.Host}";
        var loginUrl = $"{websiteUrl.TrimEnd('/')}/Account/Login";
        var safeEmail = WebUtility.HtmlEncode(email);
        var safePassword = WebUtility.HtmlEncode(password);
        var safeClubName = WebUtility.HtmlEncode(clubName);
        var safeWebsiteUrl = WebUtility.HtmlEncode(websiteUrl);

        using var client = new SmtpClient(host, port)
        {
            EnableSsl = enableSsl,
            Credentials = string.IsNullOrWhiteSpace(userName)
                ? CredentialCache.DefaultNetworkCredentials
                : new NetworkCredential(userName, passwordValue)
        };

        using var message = new MailMessage
        {
            From = new MailAddress(fromAddress),
            Subject = $"Welcome to {clubName} — your account is ready",
            Body = BuildWelcomeEmailBody(safeEmail, safePassword, loginUrl, safeClubName, safeWebsiteUrl),
            IsBodyHtml = true
        };
        message.To.Add(email);

        try
        {
            await client.SendMailAsync(message);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to send welcome email to {Email}.", email);
            return false;
        }
    }

    private static string BuildWelcomeEmailBody(string email, string password, string loginUrl, string clubName, string websiteUrl)
    {
        return $@"<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
  <title>Welcome to {clubName}</title>
  <style>
    body {{ font-family: Arial, Helvetica, sans-serif; margin: 0; padding: 0; background: #f4f7fb; color: #18324a; }}
    .wrapper {{ padding: 32px 16px; }}
    .card {{ max-width: 640px; margin: 0 auto; background: #ffffff; border-radius: 24px; overflow: hidden; box-shadow: 0 16px 40px rgba(24, 50, 74, 0.12); }}
    .hero {{ background: linear-gradient(135deg, #0f6c8a 0%, #2c9fb7 100%); padding: 36px 32px; color: #ffffff; }}
    .hero h1 {{ margin: 0 0 10px; font-size: 28px; }}
    .hero p {{ margin: 0; font-size: 15px; line-height: 1.6; opacity: 0.95; }}
    .brand-badge {{ display: inline-block; padding: 8px 12px; border-radius: 999px; background: rgba(255,255,255,0.18); font-size: 12px; font-weight: 700; letter-spacing: 0.08em; text-transform: uppercase; margin-bottom: 14px; }}
    .content {{ padding: 32px; }}
    .content p {{ font-size: 15px; line-height: 1.7; color: #425466; }}
    .credentials {{ margin: 24px 0; display: grid; gap: 12px; }}
    .credential {{ border: 1px solid #e4ebf2; border-radius: 14px; padding: 14px 16px; background: #f9fbfe; }}
    .credential span {{ display: block; font-size: 12px; text-transform: uppercase; letter-spacing: 0.08em; color: #7f8fa3; margin-bottom: 4px; }}
    .credential strong {{ font-size: 15px; color: #18324a; word-break: break-all; }}
    .button {{ display: inline-block; margin-top: 6px; padding: 13px 22px; background: #0f6c8a; color: #ffffff; text-decoration: none; border-radius: 999px; font-weight: 700; }}
    .button:hover {{ background: #0b5672; }}
    .footer {{ margin-top: 24px; font-size: 13px; color: #7f8fa3; }}
    .footer a {{ color: #0f6c8a; text-decoration: none; }}
    @media (max-width: 600px) {{ .hero, .content {{ padding: 24px; }} .hero h1 {{ font-size: 24px; }} }}
  </style>
</head>
<body>
  <div class=""wrapper"">
    <div class=""card"">
      <div class=""hero"">
        <div class=""brand-badge"">{clubName}</div>
        <h1>Welcome aboard</h1>
        <p>Your account is now ready for the {clubName} community.</p>
      </div>
      <div class=""content"">
        <p>Hi {email},</p>
        <p>We’re delighted to welcome you to <strong>{clubName}</strong>. Your account has been created and is ready to use.</p>
        <div class=""credentials"">
          <div class=""credential"">
            <span>Email</span>
            <strong>{email}</strong>
          </div>
          <div class=""credential"">
            <span>Temporary password</span>
            <strong>{password}</strong>
          </div>
        </div>
        <p>Please sign in and change your password immediately to keep your account secure.</p>
        <a href=""{loginUrl}"" class=""button"">Sign in to your account</a>
        <p class=""footer"">Discover more, connect with members, and stay in the loop at <a href=""{websiteUrl}"">{websiteUrl}</a>.</p>
      </div>
    </div>
  </div>
</body>
</html>";
    }
}

public class MembersListViewModel
{
    public IList<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    public string? Search { get; set; }
    public int? GroupId { get; set; }
    public string? Tab { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}

public class CreateMemberViewModel
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Bio { get; set; }
    public int? GroupId { get; set; }
}

public class EditMemberViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? NewPassword { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? DisplayName { get; set; }
    public string? Bio { get; set; }
    public bool IsActive { get; set; }
    public int? GroupId { get; set; }
}
