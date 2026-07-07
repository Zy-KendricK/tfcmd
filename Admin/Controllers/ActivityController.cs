using AppCore;
using AppCore.Entities;
using AppCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Admin.Controllers;

[Authorize]
public class ActivityController : BaseAdminController
{
    private readonly ApplicationDbContext _context;

    public ActivityController(
        ApplicationDbContext context,
        IPermissionService permissionService) : base(permissionService)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? search, string? type, int page = 1, int pageSize = 20)
    {
        var query = _context.Activities
            .Include(a => a.User)
            .Where(a => !a.IsDeleted)
            .AsQueryable();

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

        var viewModel = new ActivityListViewModel
        {
            Activities = activities,
            Search = search,
            Type = type,
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };

        return View(viewModel);
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
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}
