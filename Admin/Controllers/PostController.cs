using AppCore;
using AppCore.Entities;
using AppCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Admin.Controllers;

[Authorize]
[RequirePermission("posts.view")]
public class BlogController : BaseAdminController
{
    private readonly ApplicationDbContext _context;
    private readonly IContentService<Post> _postService;

    public BlogController(
        ApplicationDbContext context,
        IPermissionService permissionService) : base(permissionService)
    {
        _context = context;
        _postService = new ContentService<Post>(context);
    }

    public async Task<IActionResult> Index(string? search, ContentStatus? status, int page = 1, int pageSize = 20)
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

        var totalCount = await query.CountAsync();
        var posts = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.PendingReviewCount = await _context.Posts.CountAsync(p => p.Status == ContentStatus.PendingReview && !p.IsDeleted);

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

        return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        var post = await _context.Posts
            .Include(p => p.Author)
            .Include(p => p.Images)
            .Include(p => p.Tags)
                .ThenInclude(pt => pt.Tag)
            .Include(p => p.Comments)
                .ThenInclude(c => c.User)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null)
            return NotFound();

        return View(post);
    }

    [RequirePermission("posts.create")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Tags = await _context.Tags.Where(t => t.IsActive && !t.IsDeleted).ToListAsync();
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
            return View(model);
        }

        var post = new Post
        {
            Title = model.Title,
            Slug = GenerateSlug(model.Title),
            Excerpt = model.Excerpt,
            Content = model.Content,
            Format = model.Format,
            FeaturedImageUrl = model.FeaturedImageUrl,
            AuthorId = CurrentUserId!,
            IsFeatured = model.IsFeatured,
            AllowComments = model.AllowComments
        };

        await _postService.CreateAsync(post, CurrentUserId!);

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
            Excerpt = post.Excerpt,
            Content = post.Content,
            Format = post.Format,
            FeaturedImageUrl = post.FeaturedImageUrl,
            IsFeatured = post.IsFeatured,
            AllowComments = post.AllowComments,
            TagIds = post.Tags.Select(t => t.TagId).ToList()
        };

        ViewBag.Tags = await _context.Tags.Where(t => t.IsActive && !t.IsDeleted).ToListAsync();
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
            return View(model);
        }

        var post = await _context.Posts
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Id == model.Id);

        if (post == null)
            return NotFound();

        post.Title = model.Title;
        post.Excerpt = model.Excerpt;
        post.Content = model.Content;
        post.Format = model.Format;
        post.FeaturedImageUrl = model.FeaturedImageUrl;
        post.IsFeatured = model.IsFeatured;
        post.AllowComments = model.AllowComments;

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
        await _postService.RequestPublishAsync(id, CurrentUserId!);
        TempData["Success"] = "Publish request submitted.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("posts.publish")]
    public async Task<IActionResult> PublishToWeb(int id)
    {
        await _postService.PublishToWebAsync(id, CurrentUserId!);
        TempData["Success"] = "Post published to web.";
        return RedirectToAction(nameof(Details), new { id });
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
    public string? Excerpt { get; set; }
    public string? Content { get; set; }
    public PostFormat Format { get; set; } = PostFormat.Standard;
    public string? FeaturedImageUrl { get; set; }
    public bool IsFeatured { get; set; }
    public bool AllowComments { get; set; } = true;
    public List<int>? TagIds { get; set; }
}
