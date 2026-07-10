using AppCore;
using AppCore.Entities;
using AppCore.Services;
using Admin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Admin.Controllers;

[Authorize]
[RequirePermission("posts.view", "posts.publish", "posts.create", "posts.edit", "posts.delete")]
public class AnnouncementsController : BaseAdminController
{
    /// <summary>Maximum number of priority announcements allowed on the notice board.</summary>
    public const int PriorityCap = 12;

    private readonly ApplicationDbContext _context;
    private readonly IContentService<Announcement> _service;
    private readonly IImageUploadService _imageUpload;

    public AnnouncementsController(
        ApplicationDbContext context,
        IPermissionService permissionService,
        IImageUploadService imageUpload) : base(permissionService)
    {
        _context = context;
        _service = new ContentService<Announcement>(context);
        _imageUpload = imageUpload;
    }

    public async Task<IActionResult> Index(string? search, ContentStatus? status, string? tab, int page = 1, int pageSize = 10, bool ajax = false)
    {
        var query = _context.Announcements
            .Include(a => a.SocialGroup)
            .Where(a => !a.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            search = search.ToLower();
            query = query.Where(a =>
                a.Title.ToLower().Contains(search) ||
                (a.Content != null && a.Content.ToLower().Contains(search)));
        }

        if (status.HasValue)
        {
            query = query.Where(a => a.Status == status.Value);
        }

        query = tab switch
        {
            "pending" => query.Where(a => a.Status == ContentStatus.PendingReview),
            "live" => query.Where(a => a.IsPublishedToWeb),
            "priority" => query.Where(a => a.IsPriority),
            "internal" => query.Where(a => !a.IntendedForWeb),
            _ => query
        };

        var totalCount = await query.CountAsync();
        var announcements = await query
            .OrderByDescending(a => a.IsPriority)
            .ThenByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.PriorityUsed = await PriorityUsedAsync();
        ViewBag.PriorityCap = PriorityCap;
        ViewBag.PendingCount = await _context.Announcements.CountAsync(a => a.Status == ContentStatus.PendingReview && !a.IsDeleted);
        ViewBag.LiveCount = await _context.Announcements.CountAsync(a => a.IsPublishedToWeb && !a.IsDeleted);
        ViewBag.TotalCount = await _context.Announcements.CountAsync(a => !a.IsDeleted);
        ViewBag.InternalCount = await _context.Announcements.CountAsync(a => !a.IntendedForWeb && !a.IsDeleted);
        ViewBag.Tab = tab;

        var model = new AnnouncementListViewModel
        {
            Announcements = announcements,
            Search = search,
            Status = status,
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };

        Response.Headers["X-Has-More"] = (page < model.TotalPages).ToString().ToLowerInvariant();
        if (ajax)
        {
            return PartialView("_ListItems", model);
        }

        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var announcement = await _context.Announcements
            .Include(a => a.SocialGroup)
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);

        if (announcement == null)
            return NotFound();

        return View(announcement);
    }

    [RequirePermission("posts.create")]
    public async Task<IActionResult> Create()
    {
        await LoadGroupOptionsAsync();
        await LoadPriorityUsageAsync();
        return View(new AnnouncementViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("posts.create")]
    public async Task<IActionResult> Create(AnnouncementViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadGroupOptionsAsync();
            await LoadPriorityUsageAsync();
            return View(model);
        }

        if (model.IsPriority && await PriorityUsedAsync() >= PriorityCap)
        {
            ModelState.AddModelError(nameof(model.IsPriority),
                $"The notice board already has {PriorityCap} priority announcements. Remove the priority flag from one of them first — it will fall back into the chronological list.");
            await LoadGroupOptionsAsync();
            await LoadPriorityUsageAsync();
            return View(model);
        }

        var uploadedUrl = await _imageUpload.SaveAsync(model.ImageFile, "announcements");

        var announcement = new Announcement
        {
            Title = model.Title,
            TickerText = model.TickerText,
            Content = model.Content,
            ImageUrl = uploadedUrl ?? model.ImageUrl,
            IsPriority = model.IsPriority,
            ExpiresAt = model.ExpiresAt,
            SocialGroupId = model.SocialGroupId,
            IntendedForWeb = model.IntendedForWeb
        };

        await _service.CreateAsync(announcement, CurrentUserId!);

        TempData["Success"] = model.IntendedForWeb
            ? "Announcement created. Submit it for review to start the publishing flow."
            : "Announcement created as internal content — it will not appear on the website.";
        return RedirectToAction(nameof(Index));
    }
    [RequirePermission("posts.edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var announcement = await _context.Announcements.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
        if (announcement == null)
            return NotFound();

        var model = new AnnouncementViewModel
        {
            Id = announcement.Id,
            Title = announcement.Title,
            TickerText = announcement.TickerText,
            Content = announcement.Content,
            ImageUrl = announcement.ImageUrl,
            IsPriority = announcement.IsPriority,
            ExpiresAt = announcement.ExpiresAt,
            SocialGroupId = announcement.SocialGroupId,
            IntendedForWeb = announcement.IntendedForWeb
        };

        await LoadGroupOptionsAsync();
        await LoadPriorityUsageAsync();
        return View("Create", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("posts.edit")]
    public async Task<IActionResult> Edit(AnnouncementViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadGroupOptionsAsync();
            await LoadPriorityUsageAsync();
            return View("Create", model);
        }

        var announcement = await _context.Announcements.FirstOrDefaultAsync(a => a.Id == model.Id && !a.IsDeleted);
        if (announcement == null)
            return NotFound();

        if (model.IsPriority && !announcement.IsPriority && await PriorityUsedAsync() >= PriorityCap)
        {
            ModelState.AddModelError(nameof(model.IsPriority),
                $"The notice board already has {PriorityCap} priority announcements. Remove the priority flag from one of them first.");
            await LoadGroupOptionsAsync();
            await LoadPriorityUsageAsync();
            return View("Create", model);
        }

        var uploadedUrl = await _imageUpload.SaveAsync(model.ImageFile, "announcements");

        announcement.Title = model.Title;
        announcement.TickerText = model.TickerText;
        announcement.Content = model.Content;
        announcement.ImageUrl = uploadedUrl ?? model.ImageUrl;
        announcement.IsPriority = model.IsPriority;
        announcement.ExpiresAt = model.ExpiresAt;
        announcement.SocialGroupId = model.SocialGroupId;
        announcement.IntendedForWeb = model.IntendedForWeb;
        if (!model.IntendedForWeb && announcement.IsPublishedToWeb)
        {
            announcement.IsPublishedToWeb = false;
            announcement.Status = ContentStatus.Draft;
        }

        await _service.UpdateAsync(announcement, CurrentUserId!);
        await HomeContentService.TouchCacheVersionAsync(_context, CurrentUserId);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Announcement updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("posts.edit")]
    public async Task<IActionResult> TogglePriority(int id)
    {
        var announcement = await _context.Announcements.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
        if (announcement == null)
            return NotFound();

        if (!announcement.IsPriority && await PriorityUsedAsync() >= PriorityCap)
        {
            TempData["Error"] = $"The notice board already has {PriorityCap} priority announcements. Remove the priority flag from another announcement to make way — it will fall back into the chronological list.";
            return RedirectToAction(nameof(Index));
        }

        announcement.IsPriority = !announcement.IsPriority;
        announcement.UpdatedById = CurrentUserId;
        announcement.UpdatedAt = DateTime.UtcNow;
        await HomeContentService.TouchCacheVersionAsync(_context, CurrentUserId);
        await _context.SaveChangesAsync();

        TempData["Success"] = announcement.IsPriority
            ? "Announcement flagged as priority."
            : "Priority flag removed — the announcement now falls in line chronologically.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("posts.edit")]
    public async Task<IActionResult> RequestPublish(int id)
    {
        try
        {
            await _service.RequestPublishAsync(id, CurrentUserId!);
            TempData["Success"] = "Publish request submitted for review.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("posts.publish")]
    public async Task<IActionResult> Approve(int id)
    {
        await _service.ApproveAsync(id, CurrentUserId!);
        TempData["Success"] = "Announcement approved.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("posts.publish")]
    public async Task<IActionResult> Reject(int id, string? notes)
    {
        await _service.RejectAsync(id, CurrentUserId!, notes ?? "Rejected from admin list");
        TempData["Success"] = "Announcement rejected.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("posts.publish")]
    public async Task<IActionResult> PublishToWeb(int id)
    {
        try
        {
            await _service.PublishToWebAsync(id, CurrentUserId!);
            TempData["Success"] = "Announcement published to the website.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("posts.publish")]
    public async Task<IActionResult> Unpublish(int id)
    {
        await _service.UnpublishAsync(id, CurrentUserId!);
        TempData["Success"] = "Announcement removed from the website.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("posts.delete")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id, CurrentUserId!);
        TempData["Success"] = "Announcement deleted.";
        return RedirectToAction(nameof(Index));
    }

    private Task<int> PriorityUsedAsync() =>
        _context.Announcements.CountAsync(a => a.IsPriority && !a.IsDeleted && a.IsActive);

    private async Task LoadGroupOptionsAsync()
    {
        ViewBag.GroupOptions = await _context.SocialGroups
            .AsNoTracking()
            .Where(g => g.IsActive && !g.IsDeleted)
            .OrderBy(g => g.Name)
            .Select(g => new SelectListItem { Value = g.Id.ToString(), Text = g.Name })
            .ToListAsync();
    }

    private async Task LoadPriorityUsageAsync()
    {
        ViewBag.PriorityUsed = await PriorityUsedAsync();
        ViewBag.PriorityCap = PriorityCap;
    }
}

public class AnnouncementListViewModel
{
    public IList<Announcement> Announcements { get; set; } = new List<Announcement>();
    public string? Search { get; set; }
    public ContentStatus? Status { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}

public class AnnouncementViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? TickerText { get; set; }
    public string? Content { get; set; }
    public string? ImageUrl { get; set; }
    public IFormFile? ImageFile { get; set; }
    public bool IsPriority { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public int? SocialGroupId { get; set; }
    public bool IntendedForWeb { get; set; }
}
