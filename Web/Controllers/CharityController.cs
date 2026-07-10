using AppCore;
using AppCore.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Controllers
{
    public class CharityController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CharityController(ApplicationDbContext context)
        {
            _context = context;
        }

        private IQueryable<CharityProject> PublishedProjects => _context.CharityProjects
            .AsNoTracking()
            .Include(p => p.Charity)
            .Where(p => p.IsPublishedToWeb && p.IsActive && !p.IsDeleted);

        public async Task<IActionResult> Index()
        {
            var sectionItems = await _context.CharityPageItems
                .AsNoTracking()
                .Where(i => i.IsPublishedToWeb && i.IsActive && !i.IsDeleted)
                .OrderBy(i => i.DisplayOrder)
                .ThenBy(i => i.Id)
                .ToListAsync();

            var model = new CharityPageViewModel
            {
                Sections = sectionItems.GroupBy(i => i.Section)
                    .ToDictionary(g => g.Key, g => g.ToList()),
                FeaturedProjects = await PublishedProjects
                    .OrderByDescending(p => p.IsFeatured)
                    .ThenByDescending(p => p.PublishedToWebAt ?? p.CreatedAt)
                    .Take(6)
                    .ToListAsync(),
                TotalProjects = await PublishedProjects.CountAsync(),
                TotalDonors = await PublishedProjects.SumAsync(p => (int?)p.DonorCount) ?? 0,
                TotalRaised = await PublishedProjects.SumAsync(p => p.CurrentAmount) ?? 0
            };

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var project = await PublishedProjects.FirstOrDefaultAsync(p => p.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            var otherProjects = await PublishedProjects
                .Where(p => p.Id != id)
                .OrderByDescending(p => p.IsFeatured)
                .ThenByDescending(p => p.PublishedToWebAt ?? p.CreatedAt)
                .Take(3)
                .ToListAsync();

            var model = new CharityProjectDetailsViewModel
            {
                Project = project,
                OtherProjects = otherProjects
            };

            return View(model);
        }
    }

    public class CharityPageViewModel
    {
        public Dictionary<CharityPageSection, List<CharityPageItem>> Sections { get; set; } = new();
        public IList<CharityProject> FeaturedProjects { get; set; } = new List<CharityProject>();
        public int TotalProjects { get; set; }
        public int TotalDonors { get; set; }
        public decimal TotalRaised { get; set; }

        public List<CharityPageItem> ItemsFor(CharityPageSection section) =>
            Sections.TryGetValue(section, out var items) ? items : new List<CharityPageItem>();

        public CharityPageItem? FirstFor(CharityPageSection section) =>
            ItemsFor(section).FirstOrDefault();
    }

    public class CharityProjectDetailsViewModel
    {
        public CharityProject Project { get; set; } = null!;
        public IList<CharityProject> OtherProjects { get; set; } = new List<CharityProject>();
    }
}
