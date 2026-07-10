using AppCore;
using AppCore.Entities;
using AppCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Admin.Controllers;

[Authorize]
[RequirePermission("maintenance.cache")]
public class MaintenanceController : BaseAdminController
{
    private readonly ApplicationDbContext _context;
    private readonly IHomeContentService _homeContent;
    private readonly IAuditLogService _auditLog;

    public MaintenanceController(
        ApplicationDbContext context,
        IHomeContentService homeContent,
        IAuditLogService auditLog,
        IPermissionService permissionService) : base(permissionService)
    {
        _context = context;
        _homeContent = homeContent;
        _auditLog = auditLog;
    }

    public async Task<IActionResult> Index()
    {
        var model = new MaintenanceViewModel
        {
            CacheVersion = await _homeContent.GetCacheVersionAsync(),
            PublishedPosts = await _context.Posts.CountAsync(p => !p.IsDeleted && p.IsPublishedToWeb),
            PublishedJobs = await _context.Jobs.CountAsync(j => !j.IsDeleted && j.IsPublishedToWeb),
            PublishedAdverts = await _context.Adverts.CountAsync(a => !a.IsDeleted && a.IsPublishedToWeb),
            PublishedProducts = await _context.Products.CountAsync(p => !p.IsDeleted && p.IsPublishedToWeb),
            HomePagePosts = await _context.Posts.CountAsync(p => !p.IsDeleted && (p.IsFeatured || p.ShowOnHomePage) && p.HomeSection != HomeSection.None),
            LastCacheBump = await GetLastCacheBumpAsync()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ClearCache()
    {
        var newVersion = await _homeContent.BumpCacheVersionAsync(CurrentUserId!);

        await _auditLog.LogAsync(
            "maintenance.cache_cleared",
            CurrentUserId,
            "Setting",
            newValues: new { CacheVersion = newVersion });

        TempData["Success"] = "Website cache cleared. The public site will pick up fresh data within 30 seconds.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<DateTime?> GetLastCacheBumpAsync()
    {
        var setting = await _context.Settings
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Key == "site.cacheVersion");

        return setting?.UpdatedAt ?? setting?.CreatedAt;
    }
}

public class MaintenanceViewModel
{
    public string CacheVersion { get; set; } = string.Empty;
    public int PublishedPosts { get; set; }
    public int PublishedJobs { get; set; }
    public int PublishedAdverts { get; set; }
    public int PublishedProducts { get; set; }
    public int HomePagePosts { get; set; }
    public DateTime? LastCacheBump { get; set; }
}
