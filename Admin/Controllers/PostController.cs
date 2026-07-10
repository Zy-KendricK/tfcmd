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
public class BlogController : BaseAdminController
{
    private readonly ApplicationDbContext _context;
    private readonly IContentService<Post> _postService;
    private readonly IHomeContentService _homeContent;
    private readonly IImageUploadService _imageUpload;

    public BlogController(
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
            .Where(p => !p.IsDeleted)
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
            "drafts" => query.Where(p => p.Status == ContentStatus.Draft),
            "internal" => query.Where(p => !p.IntendedForWeb),
            _ => query
        };

        var totalCount = await query.CountAsync();
        var posts = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.PendingReviewCount = await _context.Posts.CountAsync(p => p.Status == ContentStatus.PendingReview && !p.IsDeleted);

        // Sidebar stats
        ViewBag.TotalPosts = await _context.Posts.CountAsync(p => !p.IsDeleted);
        ViewBag.PublishedPosts = await _context.Posts.CountAsync(p => !p.IsDeleted && p.IsPublishedToWeb);
        ViewBag.DraftPosts = await _context.Posts.CountAsync(p => !p.IsDeleted && p.Status == ContentStatus.Draft);
        ViewBag.InternalPosts = await _context.Posts.CountAsync(p => !p.IsDeleted && !p.IntendedForWeb);
        ViewBag.TagCount = await _context.Tags.CountAsync(t => t.IsActive && !t.IsDeleted);
        ViewBag.Tab = tab;

        var viewModel = new PostListViewModel
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
            .Include(p => p.Images)
            .Include(p => p.Tags)
                .ThenInclude(pt => pt.Tag)
            // Admin-side comments only: website comments for the same post stay on the website.
            .Include(p => p.Comments.Where(c => c.Source == CommentSource.Admin && !c.IsDeleted))
                .ThenInclude(c => c.User)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null)
            return NotFound();

        return View(post);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(int id, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            TempData["Error"] = "Comment cannot be empty.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        if (post == null)
            return NotFound();

        _context.PostComments.Add(new PostComment
        {
            PostId = id,
            UserId = CurrentUserId!,
            Content = content.Trim(),
            Source = CommentSource.Admin,
            IsApproved = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        TempData["Success"] = "Comment added.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("posts.edit")]
    public async Task<IActionResult> DeleteComment(int id, int commentId)
    {
        var comment = await _context.PostComments.FirstOrDefaultAsync(c => c.Id == commentId && c.PostId == id);
        if (comment == null)
            return NotFound();

        comment.IsDeleted = true;
        comment.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Comment removed.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [RequirePermission("posts.create")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Tags = await _context.Tags.Where(t => t.IsActive && !t.IsDeleted).ToListAsync();
        await LoadSectionUsageAsync();
        await LoadGroupOptionsAsync();
        return View(new PostViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("posts.create")]
    public async Task<IActionResult> Create(PostViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Tags = await _context.Tags.Where(t => t.IsActive && !t.IsDeleted).ToListAsync();
            await LoadSectionUsageAsync();
            await LoadGroupOptionsAsync();
            return View(model);
        }

        var uploadedUrl = await _imageUpload.SaveAsync(model.FeaturedImageFile, "posts");

        var post = new Post
        {
            Title = model.Title,
            Slug = string.IsNullOrWhiteSpace(model.Slug) ? GenerateSlug(model.Title) : model.Slug,
            Excerpt = model.Excerpt,
            Content = model.Content,
            Format = model.Format,
            FeaturedImageUrl = uploadedUrl ?? model.FeaturedImageUrl,
            AuthorId = CurrentUserId!,
            AllowComments = model.AllowComments,
            MetaTitle = model.MetaTitle,
            MetaDescription = model.MetaDescription,
            SocialGroupId = model.SocialGroupId,
            EventDate = model.EventDate,
            IntendedForWeb = model.IntendedForWeb
        };

        await _postService.CreateAsync(post, CurrentUserId!);

        // Apply home page placement with cap enforcement
        if (model.IsFeatured || model.ShowOnHomePage)
        {
            var placement = await _homeContent.ApplyPlacementAsync(post.Id, model.HomeSection, model.IsFeatured, model.ShowOnHomePage, CurrentUserId!);
            if (!placement.Success)
            {
                TempData["Warning"] = $"Post created, but home page placement failed: {placement.Error}";
            }
            else if (placement.Warning != null)
            {
                TempData["Warning"] = placement.Warning;
            }
        }

        // Add tags
        if (model.TagIds != null && model.TagIds.Any())
        {
            foreach (var tagId in model.TagIds)
            {
                _context.PostTags.Add(new PostTag { PostId = post.Id, TagId = tagId });
            }
            await _context.SaveChangesAsync();
        }

        TempData["Success"] = "Post created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [RequirePermission("posts.edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var post = await _context.Posts
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null)
            return NotFound();

        var model = new PostViewModel
        {
            Id = post.Id,
            Title = post.Title,
            Slug = post.Slug,
            Excerpt = post.Excerpt,
            Content = post.Content,
            Format = post.Format,
            FeaturedImageUrl = post.FeaturedImageUrl,
            IsFeatured = post.IsFeatured,
            ShowOnHomePage = post.ShowOnHomePage,
            HomeSection = post.HomeSection,
            AllowComments = post.AllowComments,
            MetaTitle = post.MetaTitle,
            MetaDescription = post.MetaDescription,
            SocialGroupId = post.SocialGroupId,
            EventDate = post.EventDate,
            IntendedForWeb = post.IntendedForWeb,
            TagIds = post.Tags.Select(t => t.TagId).ToList()
        };

        ViewBag.Tags = await _context.Tags.Where(t => t.IsActive && !t.IsDeleted).ToListAsync();
        await LoadSectionUsageAsync();
        await LoadGroupOptionsAsync();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("posts.edit")]
    public async Task<IActionResult> Edit(PostViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Tags = await _context.Tags.Where(t => t.IsActive && !t.IsDeleted).ToListAsync();
            await LoadSectionUsageAsync();
            await LoadGroupOptionsAsync();
            return View(model);
        }

        var post = await _context.Posts
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Id == model.Id);

        if (post == null)
            return NotFound();

        var uploadedUrl = await _imageUpload.SaveAsync(model.FeaturedImageFile, "posts");

        post.Title = model.Title;
        post.Slug = string.IsNullOrWhiteSpace(model.Slug) ? GenerateSlug(model.Title) : model.Slug;
        post.Excerpt = model.Excerpt;
        post.Content = model.Content;
        post.Format = model.Format;
        post.FeaturedImageUrl = uploadedUrl ?? model.FeaturedImageUrl;
        post.AllowComments = model.AllowComments;
        post.MetaTitle = model.MetaTitle;
        post.MetaDescription = model.MetaDescription;
        post.SocialGroupId = model.SocialGroupId;
        post.EventDate = model.EventDate;

        // Withdrawing web intent pulls the post out of the public workflow entirely.
        if (post.IntendedForWeb && !model.IntendedForWeb && post.IsPublishedToWeb)
        {
            post.IsPublishedToWeb = false;
            post.Status = ContentStatus.Draft;
        }
        post.IntendedForWeb = model.IntendedForWeb;

        // Update tags
        _context.PostTags.RemoveRange(post.Tags);
        if (model.TagIds != null && model.TagIds.Any())
        {
            foreach (var tagId in model.TagIds)
            {
                _context.PostTags.Add(new PostTag { PostId = post.Id, TagId = tagId });
            }
        }

        await _postService.UpdateAsync(post, CurrentUserId!);

        // Apply home page placement with cap enforcement (also handles clearing flags)
        var placement = await _homeContent.ApplyPlacementAsync(post.Id, model.HomeSection, model.IsFeatured, model.ShowOnHomePage, CurrentUserId!);
        if (!placement.Success)
        {
            TempData["Warning"] = $"Post updated, but home page placement failed: {placement.Error}";
        }
        else if (placement.Warning != null)
        {
            TempData["Warning"] = placement.Warning;
        }

        TempData["Success"] = "Post updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("posts.delete")]
    public async Task<IActionResult> Delete(int id)
    {
        await _postService.DeleteAsync(id, CurrentUserId!);
        TempData["Success"] = "Post deleted successfully.";
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
            TempData["Success"] = "Publish request submitted.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("posts.publish")]
    public async Task<IActionResult> PublishToWeb(int id)
    {
        try
        {
            await _postService.PublishToWebAsync(id, CurrentUserId!);
            TempData["Success"] = "Post published to web.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("posts.publish")]
    public async Task<IActionResult> Approve(int id)
    {
        await _postService.ApproveAsync(id, CurrentUserId!);
        TempData["Success"] = "Post approved.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("posts.publish")]
    public async Task<IActionResult> Reject(int id, string? notes)
    {
        await _postService.RejectAsync(id, CurrentUserId!, notes ?? "Rejected from admin list");
        TempData["Success"] = "Post rejected.";
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
            TempData["Success"] = "Post published to web.";
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
        TempData["Success"] = "Post removed from the website.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadSectionUsageAsync()
    {
        ViewBag.HeroUsage = await _homeContent.GetSectionUsageAsync(HomeSection.Hero);
        ViewBag.WhatsNewUsage = await _homeContent.GetSectionUsageAsync(HomeSection.WhatsNew);
        ViewBag.EventsUsage = await _homeContent.GetSectionUsageAsync(HomeSection.Events);
    }

    private async Task LoadGroupOptionsAsync()
    {
        ViewBag.GroupOptions = await _context.SocialGroups
            .AsNoTracking()
            .Where(g => g.IsActive && !g.IsDeleted)
            .OrderBy(g => g.Name)
            .Select(g => new SelectListItem { Value = g.Id.ToString(), Text = g.Name })
            .ToListAsync();
    }

    // Tags management
    public async Task<IActionResult> Tags()
    {
        var tags = await _context.Tags
            .Where(t => !t.IsDeleted)
            .OrderBy(t => t.Name)
            .ToListAsync();

        return View(tags);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("posts.create")]
    public async Task<IActionResult> CreateTag(string name)
    {
        var tag = new Tag
        {
            Name = name,
            Slug = GenerateSlug(name),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Tag created successfully.";
        return RedirectToAction(nameof(Tags));
    }

    private static string GenerateSlug(string title)
    {
        return title.ToLower()
            .Replace(" ", "-")
            .Replace("'", "")
            .Replace("\"", "");
    }
}

public class PostListViewModel
{
    public IList<Post> Posts { get; set; } = new List<Post>();
    public string? Search { get; set; }
    public ContentStatus? Status { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}

public class PostViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string? Excerpt { get; set; }
    public string? Content { get; set; }
    public PostFormat Format { get; set; } = PostFormat.Standard;
    public string? FeaturedImageUrl { get; set; }
    public IFormFile? FeaturedImageFile { get; set; }
    public bool IsFeatured { get; set; }
    public bool ShowOnHomePage { get; set; }
    public HomeSection HomeSection { get; set; } = HomeSection.None;
    public bool AllowComments { get; set; } = true;
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public int? SocialGroupId { get; set; }
    public DateTime? EventDate { get; set; }
    public bool IntendedForWeb { get; set; }
    public List<int>? TagIds { get; set; }
}
