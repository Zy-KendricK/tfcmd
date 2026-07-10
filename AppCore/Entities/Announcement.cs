using System.ComponentModel.DataAnnotations;

namespace AppCore.Entities;

/// <summary>
/// Announcement shown in the website ticker and on the notice board.
/// Follows the standard publishable workflow (draft -> review -> approve -> publish).
/// An announcement can optionally belong to a social group, in which case it
/// appears on that group's notice board instead of the site-wide board.
/// </summary>
public class Announcement : PublishableEntity
{
    [Required]
    [MaxLength(300)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Short text shown in the scrolling ticker. Falls back to Title when empty.
    /// </summary>
    [MaxLength(300)]
    public string? TickerText { get; set; }

    public string? Content { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Priority announcements are pinned on the notice board (max cap enforced by service).
    /// </summary>
    public bool IsPriority { get; set; } = false;

    /// <summary>
    /// Optional expiry after which the announcement no longer shows on the website.
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// When set, this announcement belongs to a group's notice board rather than the site-wide one.
    /// </summary>
    public int? SocialGroupId { get; set; }
    public virtual SocialGroup? SocialGroup { get; set; }
}
