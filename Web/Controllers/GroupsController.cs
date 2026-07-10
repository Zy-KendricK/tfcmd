using AppCore;
using AppCore.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Controllers
{
    public class GroupsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GroupsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private IQueryable<SocialGroup> PublishedGroups => _context.SocialGroups
            .AsNoTracking()
            .Where(g => g.IsPublishedToWeb && g.IsActive && !g.IsDeleted
                        && (g.Slug == null || g.Slug != "charity")
                        && g.Name.ToLower() != "charity");

        public async Task<IActionResult> Index()
        {
            var groups = await PublishedGroups
                .OrderBy(g => g.Name)
                .ToListAsync();

            return View(groups);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction(nameof(Index));
            }

            var group = await PublishedGroups
                .FirstOrDefaultAsync(g => g.Slug == id || g.Id.ToString() == id);

            if (group == null)
            {
                return NotFound();
            }

            var now = DateTime.UtcNow;

            var leaders = await _context.SocialGroupMembers
                .AsNoTracking()
                .Include(m => m.User)
                .Where(m => m.SocialGroupId == group.Id && !m.IsBanned && m.IsApproved
                            && m.Role != GroupMemberRole.Member)
                .OrderByDescending(m => m.Role)
                .ThenBy(m => m.JoinedAt)
                .ToListAsync();

            var memberCount = await _context.SocialGroupMembers
                .CountAsync(m => m.SocialGroupId == group.Id && !m.IsBanned && m.IsApproved);

            var groupEvents = _context.Posts
                .AsNoTracking()
                .Where(p => p.IsPublishedToWeb && p.IsActive && !p.IsDeleted
                            && p.SocialGroupId == group.Id
                            && (p.Kind == PostKind.Event || p.EventDate != null));

            var upcomingEvents = await groupEvents
                .Where(p => p.EventDate != null && p.EventDate >= now)
                .OrderBy(p => p.EventDate)
                .Take(6)
                .ToListAsync();

            var pastEvents = await groupEvents
                .Where(p => p.EventDate == null || p.EventDate < now)
                .OrderByDescending(p => p.EventDate ?? p.PublishedToWebAt ?? p.CreatedAt)
                .Take(6)
                .ToListAsync();

            var notices = await _context.Announcements
                .AsNoTracking()
                .Where(a => a.IsPublishedToWeb && a.IsActive && !a.IsDeleted
                            && a.SocialGroupId == group.Id
                            && (a.ExpiresAt == null || a.ExpiresAt > now))
                .OrderByDescending(a => a.IsPriority)
                .ThenByDescending(a => a.PublishedToWebAt ?? a.CreatedAt)
                .Take(10)
                .ToListAsync();

            var otherGroups = await PublishedGroups
                .Where(g => g.Id != group.Id)
                .OrderBy(g => g.Name)
                .ToListAsync();

            var model = new GroupPageViewModel
            {
                Group = group,
                Leaders = leaders,
                MemberCount = memberCount,
                UpcomingEvents = upcomingEvents,
                PastEvents = pastEvents,
                Notices = notices,
                OtherGroups = otherGroups
            };

            return View(model);
        }
    }

    public class GroupPageViewModel
    {
        public SocialGroup Group { get; set; } = null!;
        public IList<SocialGroupMember> Leaders { get; set; } = new List<SocialGroupMember>();
        public int MemberCount { get; set; }
        public IList<Post> UpcomingEvents { get; set; } = new List<Post>();
        public IList<Post> PastEvents { get; set; } = new List<Post>();
        public IList<Announcement> Notices { get; set; } = new List<Announcement>();
        public IList<SocialGroup> OtherGroups { get; set; } = new List<SocialGroup>();
    }
}
