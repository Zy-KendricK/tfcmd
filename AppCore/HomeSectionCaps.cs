namespace AppCore;

/// <summary>
/// Central definition of home page sections and the maximum number of items
/// each section can display. These caps mirror the exact number of slots in
/// the current website home page markup, which must never be exceeded so the
/// design, sizes, and arrangement of the page stay unchanged.
/// </summary>
public static class HomeSectionCaps
{
    /// <summary>Hero/header section: 1 featured (large) slot.</summary>
    public const int HeroFeatured = 1;

    /// <summary>Hero/header section: 3 smaller side slots.</summary>
    public const int HeroSecondary = 3;

    /// <summary>What's New section: 1 featured (large) slot.</summary>
    public const int WhatsNewFeatured = 1;

    /// <summary>What's New section: 2 smaller slots.</summary>
    public const int WhatsNewSecondary = 2;

    /// <summary>Events section: 1 featured (center, large) slot.</summary>
    public const int EventsFeatured = 1;

    /// <summary>Events section: 4 smaller side slots.</summary>
    public const int EventsSecondary = 4;

    /// <summary>Latest Posts sidebar widget: 3 slots.</summary>
    public const int LatestPosts = 3;

    /// <summary>Gallery strip: 9 slots.</summary>
    public const int Gallery = 9;

    /// <summary>Groups (categories) strip: 5 slots.</summary>
    public const int Groups = 5;

    /// <summary>
    /// Gets the maximum number of non-featured "show on home page" items for a post section.
    /// </summary>
    public static int GetSecondaryCap(AppCore.Entities.HomeSection section) => section switch
    {
        AppCore.Entities.HomeSection.Hero => HeroSecondary,
        AppCore.Entities.HomeSection.WhatsNew => WhatsNewSecondary,
        AppCore.Entities.HomeSection.Events => EventsSecondary,
        _ => 0
    };

    /// <summary>
    /// Gets the number of featured slots for a post section (always 1 for real sections).
    /// </summary>
    public static int GetFeaturedCap(AppCore.Entities.HomeSection section) => section switch
    {
        AppCore.Entities.HomeSection.Hero => HeroFeatured,
        AppCore.Entities.HomeSection.WhatsNew => WhatsNewFeatured,
        AppCore.Entities.HomeSection.Events => EventsFeatured,
        _ => 0
    };
}
