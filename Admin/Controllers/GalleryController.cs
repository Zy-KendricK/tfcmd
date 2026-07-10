using AppCore;
using AppCore.Entities;
using AppCore.Services;
using Admin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Admin.Controllers;

/// <summary>
/// Gallery management for photos AND videos (template photos-page presentation).
/// Viewing requires photos.view or videos.view; uploads, deletes and publishing
/// are checked per media type both in the UI and here in the controller.
/// </summary>
[Authorize]
public class GalleryController : BaseAdminController
{
    private readonly ApplicationDbContext _context;
    private readonly IContentService<Photo> _photoService;
    private readonly IContentService<Video> _videoService;
    private readonly IImageUploadService _imageUpload;

    public GalleryController(
        ApplicationDbContext context,
        IPermissionService permissionService,
        IImageUploadService imageUpload) : base(permissionService)
    {
        _context = context;
        _photoService = new ContentService<Photo>(context);
        _videoService = new ContentService<Video>(context);
        _imageUpload = imageUpload;
    }

    public async Task<IActionResult> Index(string type = "photos", string? search = null, string? tab = null,
        int? albumId = null, int? categoryId = null, int page = 1, int pageSize = 24)
    {
        var canPhotos = await HasPermissionAsync("photos.view");
        var canVideos = await HasPermissionAsync("videos.view");
        if (!canPhotos && !canVideos)
            return RedirectToAction("AccessDenied", "Account");

        // Fall back to whichever media type the user may actually see.
        if (type == "photos" && !canPhotos) type = "videos";
        if (type == "videos" && !canVideos) type = "photos";

        ViewBag.CanPhotos = canPhotos;
        ViewBag.CanVideos = canVideos;
        ViewBag.Type = type;
        ViewBag.Tab = tab;

        ViewBag.PhotoCount = canPhotos ? await _context.Photos.CountAsync(p => !p.IsDeleted) : 0;
        ViewBag.VideoCount = canVideos ? await _context.Videos.CountAsync(v => !v.IsDeleted) : 0;

        var model = new GalleryListViewModel
        {
            Type = type,
            Search = search,
            Tab = tab,
            AlbumId = albumId,
            CategoryId = categoryId,
            CurrentPage = page,
            PageSize = pageSize
        };

        if (type == "videos")
        {
            var query = _context.Videos
                .Include(v => v.Category)
                .Include(v => v.UploadedBy)
                .Where(v => !v.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                var s = search.ToLower();
                query = query.Where(v => v.Title.ToLower().Contains(s) ||
                    (v.Description != null && v.Description.ToLower().Contains(s)));
            }

            if (categoryId.HasValue)
                query = query.Where(v => v.CategoryId == categoryId.Value);

            query = tab switch
            {
                "pending" => query.Where(v => v.Status == ContentStatus.PendingReview),
                "live" => query.Where(v => v.IsPublishedToWeb),
                "featured" => query.Where(v => v.IsFeatured),
                "internal" => query.Where(v => !v.IntendedForWeb),
                _ => query
            };

            model.TotalCount = await query.CountAsync();
            model.Videos = await query
                .OrderByDescending(v => v.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.PendingCount = await _context.Videos.CountAsync(v => v.Status == ContentStatus.PendingReview && !v.IsDeleted);
            ViewBag.LiveCount = await _context.Videos.CountAsync(v => v.IsPublishedToWeb && !v.IsDeleted);
            ViewBag.FeaturedCount = await _context.Videos.CountAsync(v => v.IsFeatured && !v.IsDeleted);
            ViewBag.InternalCount = await _context.Videos.CountAsync(v => !v.IntendedForWeb && !v.IsDeleted);
            ViewBag.Categories = await GetVideoCategoriesSelectListAsync();
        }
        else
        {
            var query = _context.Photos
                .Include(p => p.Album)
                .Include(p => p.UploadedBy)
                .Where(p => !p.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                var s = search.ToLower();
                query = query.Where(p => p.Title.ToLower().Contains(s) ||
                    (p.Description != null && p.Description.ToLower().Contains(s)));
            }

            if (albumId.HasValue)
                query = query.Where(p => p.AlbumId == albumId.Value);

            query = tab switch
            {
                "pending" => query.Where(p => p.Status == ContentStatus.PendingReview),
                "live" => query.Where(p => p.IsPublishedToWeb),
                "featured" => query.Where(p => p.IsFeatured),
                "internal" => query.Where(p => !p.IntendedForWeb),
                _ => query
            };

            model.TotalCount = await query.CountAsync();
            model.Photos = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.PendingCount = await _context.Photos.CountAsync(p => p.Status == ContentStatus.PendingReview && !p.IsDeleted);
            ViewBag.LiveCount = await _context.Photos.CountAsync(p => p.IsPublishedToWeb && !p.IsDeleted);
            ViewBag.FeaturedCount = await _context.Photos.CountAsync(p => p.IsFeatured && !p.IsDeleted);
            ViewBag.InternalCount = await _context.Photos.CountAsync(p => !p.IntendedForWeb && !p.IsDeleted);
            ViewBag.Albums = await GetAlbumsSelectListAsync();
        }

        model.TotalPages = (int)Math.Ceiling(model.TotalCount / (double)pageSize);
        return View(model);
    }

    // ---------- Photos ----------

    [RequirePermission("photos.upload")]
    public async Task<IActionResult> UploadPhoto()
    {
        ViewBag.Albums = await GetAlbumsSelectListAsync();
        return View(new PhotoViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("photos.upload")]
    public async Task<IActionResult> UploadPhoto(PhotoViewModel model)
    {
        var uploadedUrl = await _imageUpload.SaveAsync(model.ImageFile, "gallery");
        if (uploadedUrl == null && string.IsNullOrWhiteSpace(model.Url))
        {
            ModelState.AddModelError(nameof(model.ImageFile), "Upload an image file or provide an image URL.");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Albums = await GetAlbumsSelectListAsync();
            return View(model);
        }

        var photo = new Photo
        {
            Title = model.Title,
            Slug = GenerateSlug(model.Title) + "-" + DateTime.UtcNow.Ticks % 10000,
            Description = model.Description,
            Url = uploadedUrl ?? model.Url!,
            ThumbnailUrl = uploadedUrl ?? model.Url,
            AlbumId = model.AlbumId,
            UploadedById = CurrentUserId!,
            Location = model.Location,
            TakenAt = model.TakenAt,
            IsFeatured = model.IsFeatured,
            AllowComments = model.AllowComments,
            IntendedForWeb = model.IntendedForWeb
        };

        await _photoService.CreateAsync(photo, CurrentUserId!);

        TempData["Success"] = "Photo uploaded. Submit it for review to start the publishing flow.";
        return RedirectToAction(nameof(Index));
    }

    [RequirePermission("photos.upload")]
    public async Task<IActionResult> EditPhoto(int id)
    {
        var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        if (photo == null)
            return NotFound();

        var model = new PhotoViewModel
        {
            Id = photo.Id,
            Title = photo.Title,
            Description = photo.Description,
            Url = photo.Url,
            AlbumId = photo.AlbumId,
            Location = photo.Location,
            TakenAt = photo.TakenAt,
            IsFeatured = photo.IsFeatured,
            AllowComments = photo.AllowComments,
            IntendedForWeb = photo.IntendedForWeb
        };

        ViewBag.Albums = await GetAlbumsSelectListAsync();
        return View("UploadPhoto", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("photos.upload")]
    public async Task<IActionResult> EditPhoto(PhotoViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Albums = await GetAlbumsSelectListAsync();
            return View("UploadPhoto", model);
        }

        var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == model.Id && !p.IsDeleted);
        if (photo == null)
            return NotFound();

        var uploadedUrl = await _imageUpload.SaveAsync(model.ImageFile, "gallery");

        photo.Title = model.Title;
        photo.Description = model.Description;
        if (uploadedUrl != null || !string.IsNullOrWhiteSpace(model.Url))
        {
            photo.Url = uploadedUrl ?? model.Url!;
            photo.ThumbnailUrl = uploadedUrl ?? model.Url;
        }
        photo.AlbumId = model.AlbumId;
        photo.Location = model.Location;
        photo.TakenAt = model.TakenAt;
        photo.IsFeatured = model.IsFeatured;
        photo.AllowComments = model.AllowComments;

        // Withdrawing web intent pulls the photo out of the public workflow.
        if (photo.IntendedForWeb && !model.IntendedForWeb && photo.IsPublishedToWeb)
        {
            photo.IsPublishedToWeb = false;
            photo.Status = ContentStatus.Draft;
        }
        photo.IntendedForWeb = model.IntendedForWeb;

        await _photoService.UpdateAsync(photo, CurrentUserId!);

        TempData["Success"] = "Photo updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("photos.upload")]
    public async Task<IActionResult> RequestPublishPhoto(int id)
    {
        try
        {
            await _photoService.RequestPublishAsync(id, CurrentUserId!);
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
    [RequirePermission("photos.publish")]
    public async Task<IActionResult> ApprovePhoto(int id)
    {
        await _photoService.ApproveAsync(id, CurrentUserId!);
        TempData["Success"] = "Photo approved.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("photos.publish")]
    public async Task<IActionResult> RejectPhoto(int id, string? notes)
    {
        await _photoService.RejectAsync(id, CurrentUserId!, notes ?? "Rejected from gallery");
        TempData["Success"] = "Photo rejected.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("photos.publish")]
    public async Task<IActionResult> PublishPhoto(int id)
    {
        try
        {
            await _photoService.PublishToWebAsync(id, CurrentUserId!);
            TempData["Success"] = "Photo published to the website.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("photos.publish")]
    public async Task<IActionResult> UnpublishPhoto(int id)
    {
        await _photoService.UnpublishAsync(id, CurrentUserId!);
        TempData["Success"] = "Photo removed from the website.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("photos.delete")]
    public async Task<IActionResult> DeletePhoto(int id)
    {
        await _photoService.DeleteAsync(id, CurrentUserId!);
        TempData["Success"] = "Photo deleted.";
        return RedirectToAction(nameof(Index));
    }

    // ---------- Videos ----------

    [RequirePermission("videos.upload")]
    public async Task<IActionResult> UploadVideo()
    {
        ViewBag.Categories = await GetVideoCategoriesSelectListAsync();
        return View(new VideoViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("videos.upload")]
    public async Task<IActionResult> UploadVideo(VideoViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Url))
        {
            ModelState.AddModelError(nameof(model.Url), "Provide the video URL (YouTube, Vimeo or a direct link).");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await GetVideoCategoriesSelectListAsync();
            return View(model);
        }

        var thumbnailUrl = await _imageUpload.SaveAsync(model.ThumbnailFile, "gallery");
        var (source, externalId, embedUrl) = ResolveVideoSource(model.Url!);

        var video = new Video
        {
            Title = model.Title,
            Slug = GenerateSlug(model.Title) + "-" + DateTime.UtcNow.Ticks % 10000,
            Description = model.Description,
            Url = model.Url!,
            Source = source,
            ExternalId = externalId,
            EmbedUrl = embedUrl,
            ThumbnailUrl = thumbnailUrl ?? model.ThumbnailUrl ?? DeriveExternalThumbnail(source, externalId),
            CategoryId = model.CategoryId,
            UploadedById = CurrentUserId!,
            Duration = model.DurationSeconds,
            IsFeatured = model.IsFeatured,
            AllowComments = model.AllowComments,
            IntendedForWeb = model.IntendedForWeb
        };

        await _videoService.CreateAsync(video, CurrentUserId!);

        TempData["Success"] = "Video added. Submit it for review to start the publishing flow.";
        return RedirectToAction(nameof(Index), new { type = "videos" });
    }

    [RequirePermission("videos.upload")]
    public async Task<IActionResult> EditVideo(int id)
    {
        var video = await _context.Videos.FirstOrDefaultAsync(v => v.Id == id && !v.IsDeleted);
        if (video == null)
            return NotFound();

        var model = new VideoViewModel
        {
            Id = video.Id,
            Title = video.Title,
            Description = video.Description,
            Url = video.Url,
            ThumbnailUrl = video.ThumbnailUrl,
            CategoryId = video.CategoryId,
            DurationSeconds = video.Duration,
            IsFeatured = video.IsFeatured,
            AllowComments = video.AllowComments,
            IntendedForWeb = video.IntendedForWeb
        };

        ViewBag.Categories = await GetVideoCategoriesSelectListAsync();
        return View("UploadVideo", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("videos.upload")]
    public async Task<IActionResult> EditVideo(VideoViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await GetVideoCategoriesSelectListAsync();
            return View("UploadVideo", model);
        }

        var video = await _context.Videos.FirstOrDefaultAsync(v => v.Id == model.Id && !v.IsDeleted);
        if (video == null)
            return NotFound();

        var thumbnailUrl = await _imageUpload.SaveAsync(model.ThumbnailFile, "gallery");

        video.Title = model.Title;
        video.Description = model.Description;
        if (!string.IsNullOrWhiteSpace(model.Url) && model.Url != video.Url)
        {
            var (source, externalId, embedUrl) = ResolveVideoSource(model.Url);
            video.Url = model.Url;
            video.Source = source;
            video.ExternalId = externalId;
            video.EmbedUrl = embedUrl;
        }
        video.ThumbnailUrl = thumbnailUrl ?? model.ThumbnailUrl ?? video.ThumbnailUrl;
        video.CategoryId = model.CategoryId;
        video.Duration = model.DurationSeconds;
        video.IsFeatured = model.IsFeatured;
        video.AllowComments = model.AllowComments;

        // Withdrawing web intent pulls the video out of the public workflow.
        if (video.IntendedForWeb && !model.IntendedForWeb && video.IsPublishedToWeb)
        {
            video.IsPublishedToWeb = false;
            video.Status = ContentStatus.Draft;
        }
        video.IntendedForWeb = model.IntendedForWeb;

        await _videoService.UpdateAsync(video, CurrentUserId!);

        TempData["Success"] = "Video updated.";
        return RedirectToAction(nameof(Index), new { type = "videos" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("videos.upload")]
    public async Task<IActionResult> RequestPublishVideo(int id)
    {
        try
        {
            await _videoService.RequestPublishAsync(id, CurrentUserId!);
            TempData["Success"] = "Publish request submitted for review.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Index), new { type = "videos" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("videos.publish")]
    public async Task<IActionResult> ApproveVideo(int id)
    {
        await _videoService.ApproveAsync(id, CurrentUserId!);
        TempData["Success"] = "Video approved.";
        return RedirectToAction(nameof(Index), new { type = "videos" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("videos.publish")]
    public async Task<IActionResult> RejectVideo(int id, string? notes)
    {
        await _videoService.RejectAsync(id, CurrentUserId!, notes ?? "Rejected from gallery");
        TempData["Success"] = "Video rejected.";
        return RedirectToAction(nameof(Index), new { type = "videos" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("videos.publish")]
    public async Task<IActionResult> PublishVideo(int id)
    {
        try
        {
            await _videoService.PublishToWebAsync(id, CurrentUserId!);
            TempData["Success"] = "Video published to the website.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Index), new { type = "videos" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("videos.publish")]
    public async Task<IActionResult> UnpublishVideo(int id)
    {
        await _videoService.UnpublishAsync(id, CurrentUserId!);
        TempData["Success"] = "Video removed from the website.";
        return RedirectToAction(nameof(Index), new { type = "videos" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("videos.delete")]
    public async Task<IActionResult> DeleteVideo(int id)
    {
        await _videoService.DeleteAsync(id, CurrentUserId!);
        TempData["Success"] = "Video deleted.";
        return RedirectToAction(nameof(Index), new { type = "videos" });
    }

    // ---------- Helpers ----------

    private async Task<List<SelectListItem>> GetAlbumsSelectListAsync()
    {
        return await _context.PhotoAlbums
            .AsNoTracking()
            .Where(a => a.IsActive && !a.IsDeleted)
            .OrderBy(a => a.Name)
            .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name })
            .ToListAsync();
    }

    private async Task<List<SelectListItem>> GetVideoCategoriesSelectListAsync()
    {
        return await _context.VideoCategories
            .AsNoTracking()
            .Where(c => c.IsActive && !c.IsDeleted)
            .OrderBy(c => c.Name)
            .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
            .ToListAsync();
    }

    private static (VideoSource Source, string? ExternalId, string? EmbedUrl) ResolveVideoSource(string url)
    {
        var trimmed = url.Trim();

        // youtu.be/{id} or youtube.com/watch?v={id}
        var youtubeId = TryExtractYouTubeId(trimmed);
        if (youtubeId != null)
        {
            return (VideoSource.YouTube, youtubeId, $"https://www.youtube.com/embed/{youtubeId}");
        }

        // vimeo.com/{id}
        var vimeoMatch = System.Text.RegularExpressions.Regex.Match(trimmed, @"vimeo\.com/(?:video/)?(\d+)");
        if (vimeoMatch.Success)
        {
            var id = vimeoMatch.Groups[1].Value;
            return (VideoSource.Vimeo, id, $"https://player.vimeo.com/video/{id}");
        }

        return (VideoSource.External, null, null);
    }

    private static string? TryExtractYouTubeId(string url)
    {
        var match = System.Text.RegularExpressions.Regex.Match(url,
            @"(?:youtube\.com/(?:watch\?(?:.*&)?v=|shorts/|embed/)|youtu\.be/)([A-Za-z0-9_-]{6,15})");
        return match.Success ? match.Groups[1].Value : null;
    }

    private static string? DeriveExternalThumbnail(VideoSource source, string? externalId)
    {
        return source == VideoSource.YouTube && externalId != null
            ? $"https://img.youtube.com/vi/{externalId}/hqdefault.jpg"
            : null;
    }

    private static string GenerateSlug(string title)
    {
        return title.ToLower()
            .Replace(" ", "-")
            .Replace("'", "")
            .Replace("\"", "");
    }
}

public class GalleryListViewModel
{
    public string Type { get; set; } = "photos";
    public IList<Photo> Photos { get; set; } = new List<Photo>();
    public IList<Video> Videos { get; set; } = new List<Video>();
    public string? Search { get; set; }
    public string? Tab { get; set; }
    public int? AlbumId { get; set; }
    public int? CategoryId { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}

public class PhotoViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Url { get; set; }
    public IFormFile? ImageFile { get; set; }
    public int? AlbumId { get; set; }
    public string? Location { get; set; }
    public DateTime? TakenAt { get; set; }
    public bool IsFeatured { get; set; }
    public bool AllowComments { get; set; } = true;
    public bool IntendedForWeb { get; set; }
}

public class VideoViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Url { get; set; }
    public string? ThumbnailUrl { get; set; }
    public IFormFile? ThumbnailFile { get; set; }
    public int? CategoryId { get; set; }
    public int? DurationSeconds { get; set; }
    public bool IsFeatured { get; set; }
    public bool AllowComments { get; set; } = true;
    public bool IntendedForWeb { get; set; }
}
