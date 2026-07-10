using AppCore;
using AppCore.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Controllers
{
    public class NoticeBoardController : Controller
    {
        private const int PageSize = 10;

        private readonly ApplicationDbContext _context;

        public NoticeBoardController(ApplicationDbContext context)
        {
            _context = context;
        }

        private IQueryable<Announcement> PublishedAnnouncements => _context.Announcements
            .AsNoTracking()
            .Where(a => a.IsPublishedToWeb && a.IsActive && !a.IsDeleted
                        && a.SocialGroupId == null
                        && (a.ExpiresAt == null || a.ExpiresAt > DateTime.UtcNow));

        public async Task<IActionResult> Index(int page = 1)
        {
            var regular = PublishedAnnouncements.Where(a => !a.IsPriority);

            var totalCount = await regular.CountAsync();
            var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)PageSize));
            page = Math.Clamp(page, 1, totalPages);

            var model = new NoticeBoardViewModel
            {
                Announcements = await regular
                    .OrderByDescending(a => a.PublishedToWebAt ?? a.CreatedAt)
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize)
                    .ToListAsync(),
                PriorityAnnouncements = await PublishedAnnouncements
                    .Where(a => a.IsPriority)
                    .OrderByDescending(a => a.PublishedToWebAt ?? a.CreatedAt)
                    .Take(12)
                    .ToListAsync(),
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var announcement = await PublishedAnnouncements.FirstOrDefaultAsync(a => a.Id == id);
            if (announcement == null)
            {
                return NotFound();
            }

            var model = new NoticeDetailsViewModel
            {
                Announcement = announcement,
                RecentAnnouncements = await PublishedAnnouncements
                    .Where(a => a.Id != id)
                    .OrderByDescending(a => a.PublishedToWebAt ?? a.CreatedAt)
                    .Take(5)
                    .ToListAsync()
            };

            return View(model);
        }
    }

    public class NoticeBoardViewModel
    {
        public IList<Announcement> Announcements { get; set; } = new List<Announcement>();
        public IList<Announcement> PriorityAnnouncements { get; set; } = new List<Announcement>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }

    public class NoticeDetailsViewModel
    {
        public Announcement Announcement { get; set; } = null!;
        public IList<Announcement> RecentAnnouncements { get; set; } = new List<Announcement>();
    }
}
