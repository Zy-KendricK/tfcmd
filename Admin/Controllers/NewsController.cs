using AppCore;
using AppCore.Entities;
using AppCore.Services;
using Admin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Admin.Controllers;

/// <summary>
/// Dedicated screen for News items shown on the website (home "What's New" section
/// and the News listing). News items are Posts with Kind = News and follow the same
/// review/approval flow as blog posts.
/// </summary>
[Authorize]
[RequirePermission("posts.view", "posts.publish", "posts.create", "posts.edit", "posts.delete")]
public class NewsController : BaseAdminController
{
    private readonly ApplicationDbContext _context;
    private readonly IContentService<Post> _postService;
    private readonly IHomeContentService _homeContent;
    private readonly IImageUploadService _imageUpload;

    public NewsController(
        ApplicationDbContext context,
        IPermissionService permissionService,
        IHomeContentService homeContent,
        IImageUploadService imageUpload) : base(permissionService)
    {
        _context = context;
        _postService = new ContentService<Post>(context);
        _homeContent = homeContent;
        _imageUpload = imageUpload;
    }

    public async Task<IActionResult> Index(string? search, ContentStatus? status, string? tab, int page = 1, int pageSize = 10, bool ajax = false)
    {
        var query = _context.Posts
            .Include(p => p.Author)
            .Where(p => !p.IsDeleted && p.Kind == PostKind.News)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            search = search.ToLower();
            query = query.Where(p =>
                p.Title.ToLower().Contains(search) ||
                (p.Content != null && p.Content.ToLower().Contains(search)));
        }

        if (status.HasValue)
        {
            query = query.Where(p => p.Status == status.Value);
        }

        query = tab switch
        {
            "pending" => query.Where(p => p.Status == ContentStatus.PendingReview),
            "live" => query.Where(p => p.IsPublishedToWeb),
            "internal" => query.Where(p => !p.IntendedForWeb),
            _ => query
        };

        var totalCount = await query.CountAsync();
        var posts = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.TotalNews = await _context.Posts.CountAsync(p => !p.IsDeleted && p.Kind == PostKind.News);
        ViewBag.LiveNews = await _context.Posts.CountAsync(p => !p.IsDeleted && p.Kind == PostKind.News && p.IsPublishedToWeb);
        ViewBag.PendingNews = await _context.Posts.CountAsync(p => !p.IsDeleted && p.Kind == PostKind.News && p.Status == ContentStatus.PendingReview);
        ViewBag.InternalNews = await _context.Posts.CountAsync(p => !p.IsDeleted && p.Kind == PostKind.News && !p.IntendedForWeb);
        ViewBag.Tab = tab;

        var viewModel = new NewsListViewModel
        {
            Posts = posts,
            Search = search,
            Status = status,
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

    public async Task<IActionResult> Details(int id)
    {
        var post = await _context.Posts
            .Include(p => p.Author)
            .Include(p => p.Tags)
                .ThenInclude(pt => pt.Tag)
            .FirstOrDefaultAsync(p => p.Id == id && p.Kind == PostKind.News && !p.IsDeleted);

        if (post == null)
            return NotFound();

        return View(post);
    }

    [RequirePermission("posts.create")]
    public async Task<IActionResult> Create()
    {
        await LoadSectionUsageAsync();
        return View(new NewsViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("posts.create")]
    public async Task<IActionResult> Create(NewsViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadSectionUsageAsync();
            return View(model);
        }

        var uploadedUrl = await _imageUpload.SaveAsync(model.FeaturedImageFile, "news");

        var post = new Post
        {
            Title = model.Title,
            Slug = GenerateSlug(model.Title) + "-" + DateTime.UtcNow.Ticks % 10000,
            Excerpt = model.Excerpt,
            Content = model.Content,
            Kind = PostKind.News,
            FeaturedImageUrl = uploadedUrl ?? model.FeaturedImageUrl,
            AuthorId = CurrentUserId!,
            AllowComments = model.AllowComments,
            IntendedForWeb = model.IntendedForWeb
        };

        await _postService.CreateAsync(post, CurrentUserId!);

        // News lives in the What's New home section when placed on the home page.
        if (model.IsFeatured || model.ShowOnHomePage)
        {
            var placement = await _homeContent.ApplyPlacementAsync(post.Id, HomeSection.WhatsNew, model.IsFeatured, model.ShowOnHomePage, CurrentUserId!);
            if (!placement.Success)
            {
                TempData["Warning"] = $"News item created, but home page placement failed: {placement.Error}";
            }
            else if (placement.Warning != null)
            {
                TempData["Warning"] = placement.Warning;
            }
        }

        TempData["Success"] = "News item created.";
        return RedirectToAction(nameof(Index));
    }

    [RequirePermission("posts.edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id && p.Kind == PostKind.News && !p.IsDeleted);
        if (post == null)
            return NotFound();

        var model = new NewsViewModel
        {
            Id = post.Id,
            Title = post.Title,
            Excerpt = post.Excerpt,
            Content = post.Content,
            FeaturedImageUrl = post.FeaturedImageUrl,
            IsFeatured = post.IsFeatured,
            ShowOnHomePage = post.ShowOnHomePage,
            AllowComments = post.AllowComments,
            IntendedForWeb = post.IntendedForWeb
        };

        await LoadSectionUsageAsync();
        return View("Create", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("posts.edit")]
    public async Task<IActionResult> Edit(NewsViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadSectionUsageAsync();
            return View("Create", model);
        }

        var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == model.Id && p.Kind == PostKind.News && !p.IsDeleted);
        if (post == null)
            return NotFound();

        var uploadedUrl = await _imageUpload.SaveAsync(model.FeaturedImageFile, "news");

        post.Title = model.Title;
        post.Excerpt = model.Excerpt;
        post.Content = model.Content;
        post.FeaturedImageUrl = uploadedUrl ?? model.FeaturedImageUrl;
        post.AllowComments = model.AllowComments;
        post.IntendedForWeb = model.IntendedForWeb;
        if (!model.IntendedForWeb && post.IsPublishedToWeb)
        {
            post.IsPublishedToWeb = false;
            post.Status = ContentStatus.Draft;
            TempData["Warning"] = "The news item was live on the website; withdrawing web intent has unpublished it.";
        }

        await _postService.UpdateAsync(post, CurrentUserId!);

        var placement = await _homeContent.ApplyPlacementAsync(post.Id, HomeSection.WhatsNew, model.IsFeatured, model.ShowOnHomePage, CurrentUserId!);
        if (!placement.Success)
        {
            TempData["Warning"] = $"News item updated, but home page placement failed: {placement.Error}";
        }
        else if (placement.Warning != null)
        {
            TempData["Warning"] = placement.Warning;
        }

        TempData["Success"] = "News item updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("posts.edit")]
    public async Task<IActionResult> RequestPublish(int id)
    {
        try
        {
            await _postService.RequestPublishAsync(id, CurrentUserId!);
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
        await _postService.ApproveAsync(id, CurrentUserId!);
        TempData["Success"] = "News item approved.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("posts.publish")]
    public async Task<IActionResult> Reject(int id, string? notes)
    {
        await _postService.RejectAsync(id, CurrentUserId!, notes ?? "Rejected from admin list");
        TempData["Success"] = "News item rejected.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("posts.publish")]
    public async Task<IActionResult> Publish(int id)
    {
        try
        {
            await _postService.PublishToWebAsync(id, CurrentUserId!);
            TempData["Success"] = "News item published to the website.";
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
        await _postService.UnpublishAsync(id, CurrentUserId!);
        TempData["Success"] = "News item removed from the website.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("posts.delete")]
    public async Task<IActionResult> Delete(int id)
    {
        await _postService.DeleteAsync(id, CurrentUserId!);
        TempData["Success"] = "News item deleted.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadSectionUsageAsync()
    {
        ViewBag.WhatsNewUsage = await _homeContent.GetSectionUsageAsync(HomeSection.WhatsNew);
    }

    private static string GenerateSlug(string title)
    {
        return title.ToLower()
            .Replace(" ", "-")
            .Replace("'", "")
            .Replace("\"", "");
    }
}

public class NewsListViewModel
{
    public IList<Post> Posts { get; set; } = new List<Post>();
    public string? Search { get; set; }
    public ContentStatus? Status { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}

public class NewsViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Excerpt { get; set; }
    public string? Content { get; set; }
    public string? FeaturedImageUrl { get; set; }
    public IFormFile? FeaturedImageFile { get; set; }
    public bool IsFeatured { get; set; }
    public bool ShowOnHomePage { get; set; }
    public bool AllowComments { get; set; } = true;
    public bool IntendedForWeb { get; set; }
}
