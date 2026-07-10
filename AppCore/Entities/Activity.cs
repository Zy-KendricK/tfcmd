using System.ComponentModel.DataAnnotations;

namespace AppCore.Entities;

/// <summary>
/// Activity feed item - tracks all activities in the platform
/// </summary>
public class Activity : BaseEntity
{
    [Required]
    public string UserId { get; set; } = string.Empty;
    public virtual ApplicationUser User { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string ActivityType { get; set; } = string.Empty; // post, comment, like, share, join_group, etc.

    [MaxLength(1000)]
    public string? Content { get; set; }

    /// <summary>
    /// Related entity type (Post, Job, Advert, etc.)
    /// </summary>
    [MaxLength(50)]
    public string? RelatedEntityType { get; set; }

    /// <summary>
    /// Related entity ID
    /// </summary>
    public int? RelatedEntityId { get; set; }

    /// <summary>
    /// Privacy setting: Public, Friends, Private
    /// </summary>
    public ActivityPrivacy Privacy { get; set; } = ActivityPrivacy.Public;

    // Navigation properties
    public virtual ICollection<ActivityComment> Comments { get; set; } = new List<ActivityComment>();
    public virtual ICollection<ActivityLike> Likes { get; set; } = new List<ActivityLike>();
}

public enum ActivityPrivacy
{
    Public = 0,
    Friends = 1,
    Private = 2,
    Group = 3
}

/// <summary>
/// Comment on an activity
/// </summary>
public class ActivityComment : BaseEntity
{
    public int ActivityId { get; set; }
    public virtual Activity Activity { get; set; } = null!;

    [Required]
    public string UserId { get; set; } = string.Empty;
    public virtual ApplicationUser User { get; set; } = null!;

    [Required]
    [MaxLength(2000)]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Parent comment ID for nested replies
    /// </summary>
    public int? ParentCommentId { get; set; }
    public virtual ActivityComment? ParentComment { get; set; }

    public virtual ICollection<ActivityComment> Replies { get; set; } = new List<ActivityComment>();
}

/// <summary>
/// Like on an activity
/// </summary>
public class ActivityLike
{
    public int Id { get; set; }

    public int ActivityId { get; set; }
    public virtual Activity Activity { get; set; } = null!;

    public string UserId { get; set; } = string.Empty;
    public virtual ApplicationUser User { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
