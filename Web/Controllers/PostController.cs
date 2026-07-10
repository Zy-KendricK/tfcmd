using AppCore;
using AppCore.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Controllers
{
	public class PostController : Controller
	{
		private const int PageSize = 9;

		private readonly ApplicationDbContext _context;
		private readonly ILogger<PostController> _logger;

		public PostController(ApplicationDbContext context, ILogger<PostController> logger)
		{
			_context = context;
			_logger = logger;
		}

		private IQueryable<Post> PublishedPosts => _context.Posts
			.AsNoTracking()
			.Include(p => p.Author)
			.Include(p => p.Category)
			.Where(p => p.IsPublishedToWeb && p.IsActive && !p.IsDeleted);

		public async Task<IActionResult> Details(int id)
		{
			var post = await PublishedPosts
				.Include(p => p.Tags).ThenInclude(t => t.Tag)
				.Include(p => p.Images)
				.FirstOrDefaultAsync(p => p.Id == id);

			if (post == null)
			{
				return NotFound();
			}

			var comments = await _context.PostComments
				.AsNoTracking()
				.Include(c => c.User)
				.Include(c => c.Replies.Where(r => r.IsApproved && r.IsActive && !r.IsDeleted && r.Source == CommentSource.Website))
					.ThenInclude(r => r.User)
				.Where(c => c.PostId == id && c.ParentCommentId == null && c.IsApproved && c.IsActive && !c.IsDeleted && c.Source == CommentSource.Website)
				.OrderBy(c => c.CreatedAt)
				.ToListAsync();

			var publishedAt = post.PublishedToWebAt ?? post.CreatedAt;
			var previousPost = await PublishedPosts
				.Where(p => p.Id != post.Id && (p.PublishedToWebAt ?? p.CreatedAt) < publishedAt)
				.OrderByDescending(p => p.PublishedToWebAt ?? p.CreatedAt)
				.FirstOrDefaultAsync();
			var nextPost = await PublishedPosts
				.Where(p => p.Id != post.Id && (p.PublishedToWebAt ?? p.CreatedAt) > publishedAt)
				.OrderBy(p => p.PublishedToWebAt ?? p.CreatedAt)
				.FirstOrDefaultAsync();

			var model = new PostDetailsViewModel
			{
				Post = post,
				Comments = comments,
				PreviousPost = previousPost,
				NextPost = nextPost,
				LatestPosts = await LatestPostsAsync(post.Id),
				SidebarCategories = await SidebarCategoriesAsync()
			};

			return View(model);
		}

		public async Task<IActionResult> Category(string id, int page = 1)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				return RedirectToAction(nameof(Events));
			}

			var category = await _context.Categories
				.AsNoTracking()
				.FirstOrDefaultAsync(c => (c.Slug == id || c.Id.ToString() == id) && c.IsActive && !c.IsDeleted);

			if (category == null)
			{
				return NotFound();
			}

			var query = PublishedPosts.Where(p => p.CategoryId == category.Id);
			var model = await BuildListAsync(query, category.Name, page);
			return View("List", model);
		}

		public async Task<IActionResult> Index(int page = 1)
		{
			var model = await BuildListAsync(PublishedPosts, "Posts", page);
			return View("List", model);
		}

		public async Task<IActionResult> News(int page = 1)
		{
			var query = PublishedPosts.Where(p => p.Kind == PostKind.News || p.HomeSection == HomeSection.WhatsNew);
			var model = await BuildListAsync(query, "News", page);
			return View("List", model);
		}

		public async Task<IActionResult> Events(int page = 1)
		{
			var query = PublishedPosts.Where(p => p.Kind == PostKind.Event || p.HomeSection == HomeSection.Events);
			var model = await BuildListAsync(query, "Events", page);
			return View("List", model);
		}

		public async Task<IActionResult> Search(string? q, int page = 1)
		{
			if (string.IsNullOrWhiteSpace(q))
			{
				return RedirectToAction(nameof(Index));
			}

			var term = q.Trim().ToLower();
			var query = PublishedPosts.Where(p =>
				p.Title.ToLower().Contains(term) ||
				(p.Excerpt != null && p.Excerpt.ToLower().Contains(term)) ||
				(p.Content != null && p.Content.ToLower().Contains(term)));

			var model = await BuildListAsync(query, $"Search: {q.Trim()}", page);
			return View("List", model);
		}

		private async Task<PostListPageViewModel> BuildListAsync(IQueryable<Post> query, string title, int page)
		{
			var totalCount = await query.CountAsync();
			var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)PageSize));
			page = Math.Clamp(page, 1, totalPages);

			var posts = await query
				.OrderByDescending(p => p.PublishedToWebAt ?? p.CreatedAt)
				.Skip((page - 1) * PageSize)
				.Take(PageSize)
				.ToListAsync();

			return new PostListPageViewModel
			{
				Title = title,
				Posts = posts,
				CurrentPage = page,
				TotalPages = totalPages,
				LatestPosts = await LatestPostsAsync(null),
				SidebarCategories = await SidebarCategoriesAsync()
			};
		}

		private async Task<IList<Post>> LatestPostsAsync(int? excludeId)
		{
			var query = PublishedPosts;
			if (excludeId.HasValue)
			{
				query = query.Where(p => p.Id != excludeId.Value);
			}

			return await query
				.OrderByDescending(p => p.PublishedToWebAt ?? p.CreatedAt)
				.Take(3)
				.ToListAsync();
		}

		private async Task<IList<Category>> SidebarCategoriesAsync()
		{
			return await _context.Categories
				.AsNoTracking()
				.Where(c => c.IsActive && !c.IsDeleted && c.CategoryType == "Post")
				.OrderBy(c => c.SortOrder)
				.ThenBy(c => c.Name)
				.Take(5)
				.ToListAsync();
		}
	}

	public class PostDetailsViewModel
	{
		public Post Post { get; set; } = null!;
		public IList<PostComment> Comments { get; set; } = new List<PostComment>();
		public Post? PreviousPost { get; set; }
		public Post? NextPost { get; set; }
		public IList<Post> LatestPosts { get; set; } = new List<Post>();
		public IList<Category> SidebarCategories { get; set; } = new List<Category>();
	}

	public class PostListPageViewModel
	{
		public string Title { get; set; } = string.Empty;
		public IList<Post> Posts { get; set; } = new List<Post>();
		public int CurrentPage { get; set; } = 1;
		public int TotalPages { get; set; } = 1;
		public IList<Post> LatestPosts { get; set; } = new List<Post>();
		public IList<Category> SidebarCategories { get; set; } = new List<Category>();
	}
}
