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

    public MembersController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IUserGroupService groupService,
        IUserService userService,
        IPermissionService permissionService) : base(permissionService)
    {
        _context = context;
        _userManager = userManager;
        _groupService = groupService;
        _userService = userService;
    }

    public async Task<IActionResult> Index(string? search, int page = 1, int pageSize = 20)
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

        var totalCount = await query.CountAsync();
        var users = await query
            .Include(u => u.GroupMemberships)
                .ThenInclude(m => m.UserGroup)
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var viewModel = new MembersListViewModel
        {
            Users = users,
            Search = search,
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };

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

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            DisplayName = $"{model.FirstName} {model.LastName}",
            Bio = model.Bio,
            IsActive = true,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            // Add to selected groups
            if (model.GroupIds != null && model.GroupIds.Any())
            {
                foreach (var groupId in model.GroupIds)
                {
                    await _groupService.AddUserToGroupAsync(user.Id, groupId, model.GroupIds.First() == groupId);
                }
            }

            TempData["Success"] = "Member created successfully.";
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
            GroupIds = user.GroupMemberships.Select(m => m.UserGroupId).ToList()
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

        // Update groups
        var currentGroupIds = user.GroupMemberships.Select(m => m.UserGroupId).ToList();
        var newGroupIds = model.GroupIds ?? new List<int>();

        // Remove from old groups
        foreach (var groupId in currentGroupIds.Except(newGroupIds))
        {
            await _groupService.RemoveUserFromGroupAsync(user.Id, groupId);
        }

        // Add to new groups
        foreach (var groupId in newGroupIds.Except(currentGroupIds))
        {
            await _groupService.AddUserToGroupAsync(user.Id, groupId);
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
}

public class MembersListViewModel
{
    public IList<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    public string? Search { get; set; }
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
    public List<int>? GroupIds { get; set; }
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
    public List<int>? GroupIds { get; set; }
}
