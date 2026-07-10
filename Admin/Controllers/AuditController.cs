using AppCore;
using AppCore.Entities;
using AppCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Admin.Controllers;

[Authorize]
[RequirePermission("audit.view")]
public class AuditController : BaseAdminController
{
    private readonly ApplicationDbContext _context;

    public AuditController(
        ApplicationDbContext context,
        IPermissionService permissionService) : base(permissionService)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? search, string? actionType, string? entityType, DateTime? from, DateTime? to, int page = 1, int pageSize = 25)
    {
        var query = _context.AuditLogs
            .Include(l => l.User)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(l =>
                l.Action.Contains(search) ||
                l.EntityType != null && l.EntityType.Contains(search) ||
                l.User != null && l.User.DisplayName != null && l.User.DisplayName.Contains(search) ||
                l.IpAddress != null && l.IpAddress.Contains(search));
        }

        if (!string.IsNullOrEmpty(actionType))
        {
            query = query.Where(l => l.Action == actionType);
        }

        if (!string.IsNullOrEmpty(entityType))
        {
            query = query.Where(l => l.EntityType == entityType);
        }

        if (from.HasValue)
        {
            query = query.Where(l => l.CreatedAt >= from.Value.Date);
        }

        if (to.HasValue)
        {
            var end = to.Value.Date.AddDays(1);
            query = query.Where(l => l.CreatedAt < end);
        }

        var totalCount = await query.CountAsync();
        var logs = await query
            .OrderByDescending(l => l.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Filter dropdown values
        ViewBag.Actions = await _context.AuditLogs
            .Select(l => l.Action)
            .Distinct()
            .OrderBy(a => a)
            .ToListAsync();
        ViewBag.EntityTypes = await _context.AuditLogs
            .Where(l => l.EntityType != null)
            .Select(l => l.EntityType!)
            .Distinct()
            .OrderBy(t => t)
            .ToListAsync();

        // Sidebar stats
        var today = DateTime.UtcNow.Date;
        ViewBag.TotalLogs = await _context.AuditLogs.CountAsync();
        ViewBag.TodayLogs = await _context.AuditLogs.CountAsync(l => l.CreatedAt >= today);
        ViewBag.WeekLogs = await _context.AuditLogs.CountAsync(l => l.CreatedAt >= today.AddDays(-7));
        ViewBag.ActiveUsers = await _context.AuditLogs
            .Where(l => l.CreatedAt >= today.AddDays(-7) && l.UserId != null)
            .Select(l => l.UserId)
            .Distinct()
            .CountAsync();

        var viewModel = new AuditLogListViewModel
        {
            Logs = logs,
            Search = search,
            Action = actionType,
            EntityType = entityType,
            From = from,
            To = to,
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        var log = await _context.AuditLogs
            .Include(l => l.User)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (log == null)
            return NotFound();

        return View(log);
    }
}

public class AuditLogListViewModel
{
    public IList<AuditLog> Logs { get; set; } = new List<AuditLog>();
    public string? Search { get; set; }
    public string? Action { get; set; }
    public string? EntityType { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}
