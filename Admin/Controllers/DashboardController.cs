using AppCore;
using AppCore.Entities;
using AppCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Admin.Controllers;

[Authorize]
[RequirePermission("dashboard.view")]
public class DashboardController : BaseAdminController
{
    private readonly ApplicationDbContext _context;

    public DashboardController(
        ApplicationDbContext context,
        IPermissionService permissionService) : base(permissionService)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var viewModel = new DashboardViewModel
        {
            TotalUsers = await _context.Users.CountAsync(u => !u.IsDeleted),
            TotalPosts = await _context.Posts.CountAsync(p => !p.IsDeleted),
            TotalJobs = await _context.Jobs.CountAsync(j => !j.IsDeleted),
            TotalAdverts = await _context.Adverts.CountAsync(a => !a.IsDeleted),
            TotalProducts = await _context.Products.CountAsync(p => !p.IsDeleted),
            TotalGroups = await _context.SocialGroups.CountAsync(g => !g.IsDeleted),
            TotalForumTopics = await _context.ForumTopics.CountAsync(t => !t.IsDeleted),
            TotalPhotos = await _context.Photos.CountAsync(p => !p.IsDeleted),
            TotalVideos = await _context.Videos.CountAsync(v => !v.IsDeleted),

            PendingReviewCount = await _context.Posts.CountAsync(p => p.Status == ContentStatus.PendingReview && !p.IsDeleted) +
                                await _context.Jobs.CountAsync(j => j.Status == ContentStatus.PendingReview && !j.IsDeleted) +
                                await _context.Adverts.CountAsync(a => a.Status == ContentStatus.PendingReview && !a.IsDeleted) +
                                await _context.Products.CountAsync(p => p.Status == ContentStatus.PendingReview && !p.IsDeleted),

            RecentActivities = await _context.Activities
                .Include(a => a.User)
                .Where(a => !a.IsDeleted)
                .OrderByDescending(a => a.CreatedAt)
                .Take(10)
                .ToListAsync(),

            RecentPosts = await _context.Posts
                .Include(p => p.Author)
                .Where(p => !p.IsDeleted)
                .OrderByDescending(p => p.CreatedAt)
                .Take(5)
                .ToListAsync(),

            NewUsersToday = await _context.Users
                .CountAsync(u => u.CreatedAt.Date == DateTime.UtcNow.Date && !u.IsDeleted),

            UnreadMessages = await _context.ContactMessages
                .CountAsync(m => !m.IsRead && !m.IsDeleted)
        };

        return View(viewModel);
    }
}

public class DashboardViewModel
{
    public int TotalUsers { get; set; }
    public int TotalPosts { get; set; }
    public int TotalJobs { get; set; }
    public int TotalAdverts { get; set; }
    public int TotalProducts { get; set; }
    public int TotalGroups { get; set; }
    public int TotalForumTopics { get; set; }
    public int TotalPhotos { get; set; }
    public int TotalVideos { get; set; }
    public int PendingReviewCount { get; set; }
    public int NewUsersToday { get; set; }
    public int UnreadMessages { get; set; }
    public IList<Activity> RecentActivities { get; set; } = new List<Activity>();
    public IList<Post> RecentPosts { get; set; } = new List<Post>();
}
