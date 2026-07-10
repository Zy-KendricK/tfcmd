using AppCore.Services;
using Microsoft.Extensions.Caching.Memory;

namespace Web.Services;

/// <summary>
/// Read-side wrapper around <see cref="IHomeContentService"/> that caches the
/// home page content in memory. A DB-backed cache version stamp (bumped from
/// the Admin "Site Maintenance" screen) invalidates the cache so the website
/// picks up fresh data without redeploying.
/// </summary>
public interface ICachedHomeContentService
{
    Task<HomePageContent> GetHomePageContentAsync();
}

public class CachedHomeContentService : ICachedHomeContentService
{
    private const string ContentCacheKey = "web.home.content";
    private const string VersionCacheKey = "web.home.cacheVersion";
    private static readonly TimeSpan VersionCheckInterval = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan ContentLifetime = TimeSpan.FromHours(12);

    private readonly IHomeContentService _homeContent;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CachedHomeContentService> _logger;

    public CachedHomeContentService(
        IHomeContentService homeContent,
        IMemoryCache cache,
        ILogger<CachedHomeContentService> logger)
    {
        _homeContent = homeContent;
        _cache = cache;
        _logger = logger;
    }

    public async Task<HomePageContent> GetHomePageContentAsync()
    {
        var currentVersion = await GetCurrentVersionAsync();

        if (_cache.TryGetValue<(string Version, HomePageContent Content)>(ContentCacheKey, out var cached)
            && cached.Version == currentVersion)
        {
            return cached.Content;
        }

        try
        {
            var content = await _homeContent.GetHomePageContentAsync();
            _cache.Set(ContentCacheKey, (currentVersion, content), ContentLifetime);
            _logger.LogInformation("Home page content reloaded from database (cache version {Version})", currentVersion);
            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load home page content from database");

            // Serve stale content if we have it; otherwise an empty model so the page still renders.
            if (cached.Content is not null)
            {
                return cached.Content;
            }

            return new HomePageContent();
        }
    }

    private async Task<string> GetCurrentVersionAsync()
    {
        // Check the DB version stamp at most every 30 seconds to keep the home page fast.
        if (_cache.TryGetValue<string>(VersionCacheKey, out var version) && version is not null)
        {
            return version;
        }

        try
        {
            version = await _homeContent.GetCacheVersionAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not read cache version from database; using last known content");
            version = "unknown";
        }

        _cache.Set(VersionCacheKey, version, VersionCheckInterval);
        return version;
    }
}
