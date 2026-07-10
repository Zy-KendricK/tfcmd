using AppCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AppCore.Services;

/// <summary>
/// Result of a home page placement change request.
/// </summary>
public record HomePlacementResult(bool Success, string? Error = null, string? Warning = null);

/// <summary>
/// Everything the website home page needs, loaded from the database.
/// </summary>
public class HomePageContent
{
    public Post? HeroFeatured { get; set; }
    public IList<Post> HeroSecondary { get; set; } = new List<Post>();
    public Post? WhatsNewFeatured { get; set; }
    public IList<Post> WhatsNewSecondary { get; set; } = new List<Post>();
    public Post? EventsFeatured { get; set; }
    public IList<Post> EventsSecondary { get; set; } = new List<Post>();
    public IList<Post> LatestPosts { get; set; } = new List<Post>();
    public IList<Photo> GalleryPhotos { get; set; } = new List<Photo>();
    public IList<Category> HomeCategories { get; set; } = new List<Category>();
    public IList<Category> SidebarCategories { get; set; } = new List<Category>();

    /// <summary>All published website groups (excluding charity); the home page picks 3 at random per render.</summary>
    public IList<SocialGroup> HomeGroups { get; set; } = new List<SocialGroup>();

    /// <summary>Active published announcements for the scrolling ticker, newest first.</summary>
    public IList<Announcement> Announcements { get; set; } = new List<Announcement>();
}

/// <summary>
/// Manages home page placement flags (featured / show-on-home-page) with
/// section caps, and loads the website home page content from the database.
/// Caps mirror the exact slot counts of the current home page markup so the
/// front-end design never changes.
/// </summary>
public interface IHomeContentService
{
    /// <summary>Sets or clears the featured flag for a post within its home section. Only one featured item is allowed per section; featuring un-features the previous one.</summary>
    Task<HomePlacementResult> SetFeaturedAsync(int postId, HomeSection section, bool featured, string userId);

    /// <summary>Sets or clears the show-on-home-page flag for a post within its home section, enforcing the section cap.</summary>
    Task<HomePlacementResult> SetShowOnHomePageAsync(int postId, HomeSection section, bool show, string userId);

    /// <summary>Applies both placement flags for a post (used by admin edit screens).</summary>
    Task<HomePlacementResult> ApplyPlacementAsync(int postId, HomeSection section, bool featured, bool showOnHomePage, string userId);

    /// <summary>Loads all published home page content honoring flags and caps.</summary>
    Task<HomePageContent> GetHomePageContentAsync();

    /// <summary>Gets the current cache version stamp used by the website.</summary>
    Task<string> GetCacheVersionAsync();

    /// <summary>Bumps the cache version stamp forcing the website to reload data.</summary>
    Task<string> BumpCacheVersionAsync(string userId);

    /// <summary>Gets current placement usage for a section (featured count, home count, caps) for admin UIs.</summary>
    Task<(int FeaturedUsed, int SecondaryUsed, int FeaturedCap, int SecondaryCap)> GetSectionUsageAsync(HomeSection section);
}

public class HomeContentService : IHomeContentService
{
    public const string CacheVersionKey = "site.cacheVersion";

    private readonly ApplicationDbContext _context;
    private readonly ILogger<HomeContentService>? _logger;

    public HomeContentService(ApplicationDbContext context, ILogger<HomeContentService>? logger = null)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Increments the site cache version stamp on the tracked context WITHOUT saving,
    /// so callers can persist it atomically with their own content changes. The website
    /// polls this stamp (every ~30s) and reloads home content when it changes.
    /// </summary>
    public static async Task TouchCacheVersionAsync(ApplicationDbContext context, string? userId)
    {
        var setting = await context.Settings.FirstOrDefaultAsync(s => s.Key == CacheVersionKey && !s.IsDeleted);
        if (setting == null)
        {
            setting = new Setting
            {
                Key = CacheVersionKey,
                Value = "0",
                Group = "System",
                ValueType = SettingValueType.Number,
                Description = "Website cache version stamp; bumping it forces the website to reload data from the database",
                IsActive = true
            };
            context.Settings.Add(setting);
        }

        var version = long.TryParse(setting.Value, out var v) ? v : 0;
        setting.Value = (version + 1).ToString();
        setting.UpdatedById = userId;
        setting.UpdatedAt = DateTime.UtcNow;
    }

    public async Task<HomePlacementResult> SetFeaturedAsync(int postId, HomeSection section, bool featured, string userId)
    {
        var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId && !p.IsDeleted);
        if (post == null)
        {
            return new HomePlacementResult(false, "Post not found.");
        }

        string? warning = null;

        if (featured)
        {
            if (section == HomeSection.None)
            {
                return new HomePlacementResult(false, "Select a home page section before featuring an item.");
            }

            // Single-featured rule: un-feature any other post in this section.
            var currentFeatured = await _context.Posts
                .Where(p => p.Id != postId && p.HomeSection == section && p.IsFeatured && !p.IsDeleted)
                .ToListAsync();

            foreach (var other in currentFeatured)
            {
                other.IsFeatured = false;
                other.UpdatedById = userId;
                warning = $"\"{other.Title}\" was un-featured because only one featured item is allowed in the {section} section.";
            }
        }

        post.IsFeatured = featured;
        post.HomeSection = section;
        post.UpdatedById = userId;

        await TouchCacheVersionAsync(_context, userId);
        await _context.SaveChangesAsync();
        _logger?.LogInformation("Post {PostId} featured={Featured} in section {Section} by {UserId}", postId, featured, section, userId);
        return new HomePlacementResult(true, Warning: warning);
    }

    public async Task<HomePlacementResult> SetShowOnHomePageAsync(int postId, HomeSection section, bool show, string userId)
    {
        var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId && !p.IsDeleted);
        if (post == null)
        {
            return new HomePlacementResult(false, "Post not found.");
        }

        if (show)
        {
            if (section == HomeSection.None)
            {
                return new HomePlacementResult(false, "Select a home page section before adding an item to the home page.");
            }

            var cap = HomeSectionCaps.GetSecondaryCap(section);
            var used = await _context.Posts
                .CountAsync(p => p.Id != postId && p.HomeSection == section && p.ShowOnHomePage && !p.IsFeatured && !p.IsDeleted);

            if (used >= cap)
            {
                return new HomePlacementResult(false,
                    $"The {section} section already has {used} of {cap} home page items. Remove one before adding another.");
            }
        }

        post.ShowOnHomePage = show;
        post.HomeSection = section;
        post.UpdatedById = userId;

        await TouchCacheVersionAsync(_context, userId);
        await _context.SaveChangesAsync();
        _logger?.LogInformation("Post {PostId} showOnHomePage={Show} in section {Section} by {UserId}", postId, show, section, userId);
        return new HomePlacementResult(true);
    }

    public async Task<HomePlacementResult> ApplyPlacementAsync(int postId, HomeSection section, bool featured, bool showOnHomePage, string userId)
    {
        var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId && !p.IsDeleted);
        if (post == null)
        {
            return new HomePlacementResult(false, "Post not found.");
        }

        string? warning = null;

        if ((featured || showOnHomePage) && section == HomeSection.None)
        {
            return new HomePlacementResult(false, "Select a home page section for this item.");
        }

        if (featured)
        {
            var currentFeatured = await _context.Posts
                .Where(p => p.Id != postId && p.HomeSection == section && p.IsFeatured && !p.IsDeleted)
                .ToListAsync();

            foreach (var other in currentFeatured)
            {
                other.IsFeatured = false;
                other.UpdatedById = userId;
                warning = $"\"{other.Title}\" was un-featured because only one featured item is allowed in the {section} section.";
            }
        }
        else if (showOnHomePage)
        {
            var cap = HomeSectionCaps.GetSecondaryCap(section);
            var used = await _context.Posts
                .CountAsync(p => p.Id != postId && p.HomeSection == section && p.ShowOnHomePage && !p.IsFeatured && !p.IsDeleted);

            if (used >= cap)
            {
                return new HomePlacementResult(false,
                    $"The {section} section already has {used} of {cap} home page items. Remove one before adding another.");
            }
        }

        post.IsFeatured = featured;
        post.ShowOnHomePage = showOnHomePage;
        post.HomeSection = (featured || showOnHomePage) ? section : HomeSection.None;
        post.UpdatedById = userId;

        await TouchCacheVersionAsync(_context, userId);
        await _context.SaveChangesAsync();
        _logger?.LogInformation("Post {PostId} placement applied: featured={Featured}, show={Show}, section={Section} by {UserId}",
            postId, featured, showOnHomePage, section, userId);
        return new HomePlacementResult(true, Warning: warning);
    }

    public async Task<HomePageContent> GetHomePageContentAsync()
    {
        var content = new HomePageContent();

        // Base query: only published, active, non-deleted posts appear on the website.
        IQueryable<Post> published = _context.Posts
            .AsNoTracking()
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Where(p => p.IsPublishedToWeb && p.IsActive && !p.IsDeleted);

        // Hero section
        content.HeroFeatured = await published
            .Where(p => p.HomeSection == HomeSection.Hero && p.IsFeatured)
            .OrderByDescending(p => p.PublishedToWebAt)
            .FirstOrDefaultAsync();
        content.HeroSecondary = await published
            .Where(p => p.HomeSection == HomeSection.Hero && !p.IsFeatured && p.ShowOnHomePage)
            .OrderByDescending(p => p.PublishedToWebAt)
            .Take(HomeSectionCaps.HeroSecondary)
            .ToListAsync();

        // What's New section
        content.WhatsNewFeatured = await published
            .Where(p => p.HomeSection == HomeSection.WhatsNew && p.IsFeatured)
            .OrderByDescending(p => p.PublishedToWebAt)
            .FirstOrDefaultAsync();
        content.WhatsNewSecondary = await published
            .Where(p => p.HomeSection == HomeSection.WhatsNew && !p.IsFeatured && p.ShowOnHomePage)
            .OrderByDescending(p => p.PublishedToWebAt)
            .Take(HomeSectionCaps.WhatsNewSecondary)
            .ToListAsync();

        // Events section
        content.EventsFeatured = await published
            .Where(p => p.HomeSection == HomeSection.Events && p.IsFeatured)
            .OrderByDescending(p => p.PublishedToWebAt)
            .FirstOrDefaultAsync();
        content.EventsSecondary = await published
            .Where(p => p.HomeSection == HomeSection.Events && !p.IsFeatured && p.ShowOnHomePage)
            .OrderByDescending(p => p.PublishedToWebAt)
            .Take(HomeSectionCaps.EventsSecondary)
            .ToListAsync();

        // Latest posts sidebar widget: most recent published posts regardless of section.
        content.LatestPosts = await published
            .OrderByDescending(p => p.PublishedToWebAt)
            .Take(HomeSectionCaps.LatestPosts)
            .ToListAsync();

        // Gallery strip: published photos flagged for home page, newest first.
        content.GalleryPhotos = await _context.Photos
            .AsNoTracking()
            .Where(p => p.IsPublishedToWeb && p.IsActive && !p.IsDeleted && p.ShowOnHomePage)
            .OrderByDescending(p => p.PublishedToWebAt)
            .Take(HomeSectionCaps.Gallery)
            .ToListAsync();

        // Groups strip: categories flagged for home page.
        content.HomeCategories = await _context.Categories
            .AsNoTracking()
            .Where(c => c.IsActive && !c.IsDeleted && c.ShowOnHomePage)
            .OrderBy(c => c.SortOrder)
            .Take(HomeSectionCaps.Groups)
            .ToListAsync();

        // Website groups: all published groups excluding charity; the page shows 3 at random.
        content.HomeGroups = await _context.SocialGroups
            .AsNoTracking()
            .Where(g => g.IsPublishedToWeb && g.IsActive && !g.IsDeleted
                        && (g.Slug == null || g.Slug != "charity")
                        && g.Name.ToLower() != "charity")
            .OrderBy(g => g.Name)
            .ToListAsync();

        // Announcement ticker: published, active, unexpired announcements (site-wide only).
        var now = DateTime.UtcNow;
        content.Announcements = await _context.Announcements
            .AsNoTracking()
            .Where(a => a.IsPublishedToWeb && a.IsActive && !a.IsDeleted
                        && a.SocialGroupId == null
                        && (a.ExpiresAt == null || a.ExpiresAt > now))
            .OrderByDescending(a => a.IsPriority)
            .ThenByDescending(a => a.PublishedToWebAt ?? a.CreatedAt)
            .Take(20)
            .ToListAsync();

        // Sidebar category list: active post categories with counts.
        content.SidebarCategories = await _context.Categories
            .AsNoTracking()
            .Where(c => c.IsActive && !c.IsDeleted && c.CategoryType == "Post")
            .OrderBy(c => c.SortOrder)
            .Take(5)
            .ToListAsync();

        return content;
    }

    public async Task<string> GetCacheVersionAsync()
    {
        var setting = await _context.Settings
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Key == CacheVersionKey && !s.IsDeleted);
        return setting?.Value ?? "1";
    }

    public async Task<string> BumpCacheVersionAsync(string userId)
    {
        await TouchCacheVersionAsync(_context, userId);
        await _context.SaveChangesAsync();

        var setting = await _context.Settings
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Key == CacheVersionKey && !s.IsDeleted);
        _logger?.LogInformation("Website cache version bumped to {Version} by {UserId}", setting?.Value, userId);
        return setting?.Value ?? "1";
    }

    public async Task<(int FeaturedUsed, int SecondaryUsed, int FeaturedCap, int SecondaryCap)> GetSectionUsageAsync(HomeSection section)
    {
        var featuredUsed = await _context.Posts
            .CountAsync(p => p.HomeSection == section && p.IsFeatured && !p.IsDeleted);
        var secondaryUsed = await _context.Posts
            .CountAsync(p => p.HomeSection == section && p.ShowOnHomePage && !p.IsFeatured && !p.IsDeleted);

        return (featuredUsed, secondaryUsed, HomeSectionCaps.GetFeaturedCap(section), HomeSectionCaps.GetSecondaryCap(section));
    }
}
