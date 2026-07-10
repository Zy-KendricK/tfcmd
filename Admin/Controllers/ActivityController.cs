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
public class ActivityController : BaseAdminController
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ActivityController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IPermissionService permissionService) : base(permissionService)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(string? search, string? type, string? scope, int page = 1, int pageSize = 20)
    {
        var query = _context.Activities
            .Include(a => a.User)
            .Include(a => a.Comments)
            .Include(a => a.Likes)
            .Where(a => !a.IsDeleted)
            .AsQueryable();

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (scope == "mentions" && !string.IsNullOrEmpty(currentUserId))
        {
            var me = await _userManager.FindByIdAsync(currentUserId);
            var handle = me?.UserName ?? "";
            query = query.Where(a => a.Content != null && a.Content.Contains("@" + handle));
        }
        else if (scope == "mine" && !string.IsNullOrEmpty(currentUserId))
        {
            query = query.Where(a => a.UserId == currentUserId);
        }

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(a =>
                a.Content != null && a.Content.Contains(search) ||
                a.User.DisplayName != null && a.User.DisplayName.Contains(search));
        }

        if (!string.IsNullOrEmpty(type))
        {
            query = query.Where(a => a.ActivityType == type);
        }

        var totalCount = await query.CountAsync();
        var activities = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Counts for the directory nav tabs (template: activity-type-navs)
        ViewBag.AllCount = await _context.Activities.CountAsync(a => !a.IsDeleted);
        ViewBag.MineCount = string.IsNullOrEmpty(currentUserId)
            ? 0
            : await _context.Activities.CountAsync(a => !a.IsDeleted && a.UserId == currentUserId);

        // Distinct types present, for the filter dropdown
        ViewBag.ActivityTypes = await _context.Activities
            .Where(a => !a.IsDeleted)
            .Select(a => a.ActivityType)
            .Distinct()
            .OrderBy(t => t)
            .ToListAsync();

        if (!string.IsNullOrEmpty(currentUserId))
        {
            var me2 = await _userManager.FindByIdAsync(currentUserId);
            ViewBag.MyAvatar = me2?.AvatarUrl ?? "/wp-content/uploads/2020/10/avatar.png";
        }

        var viewModel = new ActivityListViewModel
        {
            Activities = activities,
            Search = search,
            Type = type,
            Scope = scope,
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PostUpdate(string? content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            TempData["Error"] = "Please write something before posting.";
            return RedirectToAction(nameof(Index));
        }

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId))
            return Challenge();

        _context.Activities.Add(new Activity
        {
            UserId = currentUserId,
            ActivityType = "StatusUpdate",
            Content = content.Trim(),
            Privacy = ActivityPrivacy.Public,
            CreatedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        TempData["Success"] = "Update posted.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var activity = await _context.Activities
            .Include(a => a.User)
            .Include(a => a.Comments)
                .ThenInclude(c => c.User)
            .Include(a => a.Likes)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (activity == null)
            return NotFound();

        return View(activity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var activity = await _context.Activities.FindAsync(id);
        if (activity == null)
            return NotFound();

        activity.IsDeleted = true;
        activity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Activity deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}

public class ActivityListViewModel
{
    public IList<Activity> Activities { get; set; } = new List<Activity>();
    public string? Search { get; set; }
    public string? Type { get; set; }
    public string? Scope { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}
