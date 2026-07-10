using AppCore;
using AppCore.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.ViewComponents
{
    public class MegaMenuViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public MegaMenuViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .Where(c => c.IsActive && !c.IsDeleted && c.CategoryType == "Post")
                .OrderBy(c => c.SortOrder)
                .ThenBy(c => c.Name)
                .Take(5)
                .ToListAsync();

            var latestPosts = await _context.Posts
                .AsNoTracking()
                .Where(p => p.IsPublishedToWeb && p.IsActive && !p.IsDeleted)
                .OrderByDescending(p => p.PublishedToWebAt ?? p.CreatedAt)
                .Take(6)
                .ToListAsync();

            var categoryIds = categories.Select(c => c.Id).ToList();
            var categoryPosts = await _context.Posts
                .AsNoTracking()
                .Where(p => p.IsPublishedToWeb && p.IsActive && !p.IsDeleted &&
                            p.CategoryId != null && categoryIds.Contains(p.CategoryId.Value))
                .OrderByDescending(p => p.PublishedToWebAt ?? p.CreatedAt)
                .ToListAsync();

            var model = new MegaMenuViewModel
            {
                Categories = categories,
                LatestPosts = latestPosts,
                PostsByCategory = categories.ToDictionary(
                    c => c.Id,
                    c => (IList<Post>)categoryPosts.Where(p => p.CategoryId == c.Id).Take(6).ToList())
            };

            return View(model);
        }
    }

    public class MegaMenuViewModel
    {
        public IList<Category> Categories { get; set; } = new List<Category>();
        public IList<Post> LatestPosts { get; set; } = new List<Post>();
        public Dictionary<int, IList<Post>> PostsByCategory { get; set; } = new();
    }
}
