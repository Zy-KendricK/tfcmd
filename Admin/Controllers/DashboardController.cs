using AppCore;
using AppCore.Entities;
using AppCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Admin.Controllers;

[Authorize]
[RequirePermission("dashboard.view")]
public class DashboardController : BaseAdminController
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public DashboardController(
        ApplicationDbContext context,
        IPermissionService permissionService,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager) : base(permissionService)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<IActionResult> Index()
    {
        var viewModel = new DashboardViewModel
        {
            TotalUsers = await _context.Users.CountAsync(u => !u.IsDeleted),
            TotalPosts = await _context.Posts.CountAsync(p => !p.IsDeleted),
            TotalJobs = await _context.Jobs.CountAsync(j => !j.IsDeleted),
            TotalAdverts = await _context.Adverts.CountAsync(a => !a.IsDeleted),
            TotalProducts = await _context.Products.CountAsync(p => !p.IsDeleted),
            TotalGroups = await _context.SocialGroups.CountAsync(g => !g.IsDeleted),
            TotalForumTopics = await _context.ForumTopics.CountAsync(t => !t.IsDeleted),
            TotalPhotos = await _context.Photos.CountAsync(p => !p.IsDeleted),
            TotalVideos = await _context.Videos.CountAsync(v => !v.IsDeleted),

            PendingReviewCount = await _context.Posts.CountAsync(p => p.Status == ContentStatus.PendingReview && !p.IsDeleted) +
                                await _context.Jobs.CountAsync(j => j.Status == ContentStatus.PendingReview && !j.IsDeleted) +
                                await _context.Adverts.CountAsync(a => a.Status == ContentStatus.PendingReview && !a.IsDeleted) +
                                await _context.Products.CountAsync(p => p.Status == ContentStatus.PendingReview && !p.IsDeleted),

            RecentActivities = await _context.Activities
                .Include(a => a.User)
                .Where(a => !a.IsDeleted)
                .OrderByDescending(a => a.CreatedAt)
                .Take(10)
                .ToListAsync(),

            RecentPosts = await _context.Posts
                .Include(p => p.Author)
                .Where(p => !p.IsDeleted)
                .OrderByDescending(p => p.CreatedAt)
                .Take(5)
                .ToListAsync(),

            NewUsersToday = await _context.Users
                .CountAsync(u => u.CreatedAt.Date == DateTime.UtcNow.Date && !u.IsDeleted),

            UnreadMessages = await _context.ContactMessages
                .CountAsync(m => !m.IsRead && !m.IsDeleted)
        };

        // Recent content strip: latest posts/news (including unreleased) so every
        // admin member can consume, comment on, and react to content before it
        // reaches the public website.
        ViewBag.RecentContent = await _context.Posts
            .Include(p => p.Author)
            .Where(p => !p.IsDeleted && p.IsActive)
            .OrderByDescending(p => p.CreatedAt)
            .Take(8)
            .ToListAsync();

        // Stats used directly by the dashboard view.
        ViewBag.TotalMembers = viewModel.TotalUsers;
        ViewBag.TotalPosts = viewModel.TotalPosts;
        ViewBag.TotalJobs = viewModel.TotalJobs;
        ViewBag.TotalProducts = viewModel.TotalProducts;
        ViewBag.RecentActivities = viewModel.RecentActivities;

        ViewBag.PendingPosts = await _context.Posts.CountAsync(p => p.Status == ContentStatus.PendingReview && !p.IsDeleted);
        ViewBag.PendingJobs = await _context.Jobs.CountAsync(j => j.Status == ContentStatus.PendingReview && !j.IsDeleted);
        ViewBag.PendingAdverts = await _context.Adverts.CountAsync(a => a.Status == ContentStatus.PendingReview && !a.IsDeleted);
        ViewBag.PendingCount = viewModel.PendingReviewCount;

        ViewBag.PostsDraft = await _context.Posts.CountAsync(p => p.Status == ContentStatus.Draft && !p.IsDeleted);
        ViewBag.PostsPending = ViewBag.PendingPosts;
        ViewBag.PostsPublished = await _context.Posts.CountAsync(p => p.Status == ContentStatus.Published && !p.IsDeleted);
        ViewBag.JobsDraft = await _context.Jobs.CountAsync(j => j.Status == ContentStatus.Draft && !j.IsDeleted);
        ViewBag.JobsPending = ViewBag.PendingJobs;
        ViewBag.JobsPublished = await _context.Jobs.CountAsync(j => j.Status == ContentStatus.Published && !j.IsDeleted);
        ViewBag.AdvertsDraft = await _context.Adverts.CountAsync(a => a.Status == ContentStatus.Draft && !a.IsDeleted);
        ViewBag.AdvertsPending = ViewBag.PendingAdverts;
        ViewBag.AdvertsPublished = await _context.Adverts.CountAsync(a => a.Status == ContentStatus.Published && !a.IsDeleted);
        ViewBag.ProductsDraft = await _context.Products.CountAsync(p => p.Status == ContentStatus.Draft && !p.IsDeleted);
        ViewBag.ProductsPending = await _context.Products.CountAsync(p => p.Status == ContentStatus.PendingReview && !p.IsDeleted);
        ViewBag.ProductsPublished = await _context.Products.CountAsync(p => p.Status == ContentStatus.Published && !p.IsDeleted);
        ViewBag.TotalAdverts = viewModel.TotalAdverts;

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        var model = new Admin.Models.ProfileViewModel
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            DisplayName = user.DisplayName,
            Bio = user.Bio,
            Location = user.Location,
            Website = user.Website
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(Admin.Models.ProfileViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.DisplayName = string.IsNullOrWhiteSpace(model.DisplayName)
            ? $"{model.FirstName} {model.LastName}".Trim()
            : model.DisplayName;
        user.Bio = model.Bio;
        user.Location = model.Location;
        user.Website = model.Website;
        user.UpdatedAt = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);
        TempData["Success"] = "Profile updated successfully.";
        return RedirectToAction(nameof(Profile));
    }

    [HttpGet]
    public async Task<IActionResult> Settings(bool forceChange = false)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        ViewBag.RequiresPasswordChange = forceChange || User.HasClaim("RequirePasswordChange", "true");
        var model = new Admin.Models.SettingsViewModel
        {
            Email = user.Email ?? string.Empty
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Settings(Admin.Models.SettingsViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        ViewBag.RequiresPasswordChange = User.HasClaim("RequirePasswordChange", "true");

        if (string.IsNullOrWhiteSpace(model.Email))
        {
            ModelState.AddModelError(nameof(model.Email), "Please enter an email address.");
        }
        else
        {
            try
            {
                var parsedEmail = new System.Net.Mail.MailAddress(model.Email);
                if (!parsedEmail.Address.Equals(model.Email, StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError(nameof(model.Email), "Please enter a valid email address.");
                }
            }
            catch
            {
                ModelState.AddModelError(nameof(model.Email), "Please enter a valid email address.");
            }
        }

        if (!string.IsNullOrWhiteSpace(model.NewPassword) || !string.IsNullOrWhiteSpace(model.ConfirmPassword))
        {
            if (string.IsNullOrWhiteSpace(model.CurrentPassword))
            {
                ModelState.AddModelError(nameof(model.CurrentPassword), "Your current password is required to update your password.");
            }

            if (string.IsNullOrWhiteSpace(model.NewPassword) || string.IsNullOrWhiteSpace(model.ConfirmPassword))
            {
                ModelState.AddModelError(string.Empty, "Both new password fields are required when changing your password.");
            }
            else if (!string.Equals(model.NewPassword, model.ConfirmPassword, StringComparison.Ordinal))
            {
                ModelState.AddModelError(string.Empty, "The new password and confirmation password do not match.");
            }
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        user.Email = model.Email;
        user.UserName = model.Email;
        user.NormalizedEmail = model.Email.ToUpperInvariant();
        user.NormalizedUserName = model.Email.ToUpperInvariant();
        user.UpdatedAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        if (!string.IsNullOrWhiteSpace(model.NewPassword))
        {
            var passwordResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword!, model.NewPassword);
            if (!passwordResult.Succeeded)
            {
                foreach (var error in passwordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            await _userManager.RemoveClaimAsync(user, new Claim("RequirePasswordChange", "true"));
            await _signInManager.RefreshSignInAsync(user);
        }

        TempData["Success"] = "Account settings updated successfully.";
        return RedirectToAction(nameof(Settings));
    }
}

public class DashboardViewModel
{
    public int TotalUsers { get; set; }
    public int TotalPosts { get; set; }
    public int TotalJobs { get; set; }
    public int TotalAdverts { get; set; }
    public int TotalProducts { get; set; }
    public int TotalGroups { get; set; }
    public int TotalForumTopics { get; set; }
    public int TotalPhotos { get; set; }
    public int TotalVideos { get; set; }
    public int PendingReviewCount { get; set; }
    public int NewUsersToday { get; set; }
    public int UnreadMessages { get; set; }
    public IList<Activity> RecentActivities { get; set; } = new List<Activity>();
    public IList<Post> RecentPosts { get; set; } = new List<Post>();
}
