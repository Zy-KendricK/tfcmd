using AppCore;
using AppCore.Entities;
using AppCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Admin.Controllers;

[Authorize]
[RequirePermission("groups.view")]
public class GroupsController : BaseAdminController
{
    private readonly ApplicationDbContext _context;
    private readonly IUserGroupService _groupService;

    public GroupsController(
        ApplicationDbContext context,
        IUserGroupService groupService,
        IPermissionService permissionService) : base(permissionService)
    {
        _context = context;
        _groupService = groupService;
    }

    public async Task<IActionResult> Index()
    {
        var groups = await _context.UserGroups
            .Where(g => !g.IsDeleted)
            .Include(g => g.Members)
            .Include(g => g.Permissions)
            .OrderBy(g => g.Name)
            .ToListAsync();

        return View(groups);
    }

    public async Task<IActionResult> Details(int id)
    {
        var group = await _groupService.GetGroupByIdAsync(id);
        if (group == null)
            return NotFound();

        return View(group);
    }

    [RequirePermission("groups.manage")]
    public async Task<IActionResult> Create()
    {
        ViewBag.AllPermissions = await _permissionService.GetAllPermissionsAsync();
        return View(new GroupViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("groups.manage")]
    public async Task<IActionResult> Create(GroupViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.AllPermissions = await _permissionService.GetAllPermissionsAsync();
            return View(model);
        }

        var group = new UserGroup
        {
            Name = model.Name,
            Description = model.Description,
            IsSystemGroup = false,
            IsActive = true
        };

        await _groupService.CreateGroupAsync(group);

        if (model.PermissionIds != null && model.PermissionIds.Any())
        {
            await _groupService.SetGroupPermissionsAsync(group.Id, model.PermissionIds);
        }

        TempData["Success"] = "Group created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [RequirePermission("groups.manage")]
    public async Task<IActionResult> Edit(int id)
    {
        var group = await _groupService.GetGroupByIdAsync(id);
        if (group == null)
            return NotFound();

        var model = new GroupViewModel
        {
            Id = group.Id,
            Name = group.Name,
            Description = group.Description,
            IsSystemGroup = group.IsSystemGroup,
            PermissionIds = group.Permissions.Select(p => p.PermissionId).ToList()
        };

        ViewBag.AllPermissions = await _permissionService.GetAllPermissionsAsync();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("groups.manage")]
    public async Task<IActionResult> Edit(GroupViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.AllPermissions = await _permissionService.GetAllPermissionsAsync();
            return View(model);
        }

        var group = await _context.UserGroups.FindAsync(model.Id);
        if (group == null)
            return NotFound();

        // Don't allow editing name of system groups
        if (!group.IsSystemGroup)
        {
            group.Name = model.Name;
        }

        group.Description = model.Description;
        group.UpdatedAt = DateTime.UtcNow;

        await _groupService.UpdateGroupAsync(group);

        if (model.PermissionIds != null)
        {
            await _groupService.SetGroupPermissionsAsync(group.Id, model.PermissionIds);
        }

        TempData["Success"] = "Group updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("groups.manage")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _groupService.DeleteGroupAsync(id);
            TempData["Success"] = "Group deleted successfully.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [RequirePermission("groups.manage")]
    public async Task<IActionResult> Permissions(int id)
    {
        var group = await _groupService.GetGroupByIdAsync(id);
        if (group == null)
            return NotFound();

        var allPermissions = await _permissionService.GetAllPermissionsAsync();
        var groupPermissionIds = group.Permissions.Select(p => p.PermissionId).ToHashSet();

        var model = new GroupPermissionsViewModel
        {
            Group = group,
            PermissionsByCategory = allPermissions
                .GroupBy(p => p.Category ?? "Other")
                .ToDictionary(g => g.Key, g => g.ToList()),
            SelectedPermissionIds = groupPermissionIds
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("groups.manage")]
    public async Task<IActionResult> Permissions(int id, List<int> permissionIds)
    {
        var group = await _groupService.GetGroupByIdAsync(id);
        if (group == null)
            return NotFound();

        await _groupService.SetGroupPermissionsAsync(id, permissionIds);

        TempData["Success"] = "Permissions updated successfully.";
        return RedirectToAction(nameof(Details), new { id });
    }
}

public class GroupViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsSystemGroup { get; set; }
    public List<int>? PermissionIds { get; set; }
}

public class GroupPermissionsViewModel
{
    public UserGroup Group { get; set; } = null!;
    public Dictionary<string, List<Permission>> PermissionsByCategory { get; set; } = new();
    public HashSet<int> SelectedPermissionIds { get; set; } = new();
}
