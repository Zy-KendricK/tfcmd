using System.ComponentModel.DataAnnotations;

namespace AppCore.Entities;

/// <summary>
/// Base entity with common properties for auditing and publishing workflow
/// </summary>
public abstract class BaseEntity
{
    [Key]
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public string? CreatedById { get; set; }
    public string? UpdatedById { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
}

/// <summary>
/// Base entity for content that can be published to the website
/// </summary>
public abstract class PublishableEntity : BaseEntity
{
    /// <summary>
    /// Status of the content: Draft, PendingReview, Approved, Published, Rejected
    /// </summary>
    public ContentStatus Status { get; set; } = ContentStatus.Draft;

    /// <summary>
    /// Whether the author intends this item to appear on the public website.
    /// Items without this flag stay internal to the admin portal and can never
    /// enter the publish-to-web workflow.
    /// </summary>
    public bool IntendedForWeb { get; set; } = false;

    /// <summary>
    /// Whether the creator has requested publication
    /// </summary>
    public bool PublishRequested { get; set; } = false;
    public DateTime? PublishRequestedAt { get; set; }

    /// <summary>
    /// Whether the content is published to the website
    /// </summary>
    public bool IsPublishedToWeb { get; set; } = false;
    public DateTime? PublishedToWebAt { get; set; }
    public string? PublishedById { get; set; }

    /// <summary>
    /// Review information
    /// </summary>
    public string? ReviewedById { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewNotes { get; set; }

    /// <summary>
    /// Whether this item appears on the website home page in a non-featured slot.
    /// Featured items (IsFeatured on the concrete entity) always appear on the home page.
    /// </summary>
    public bool ShowOnHomePage { get; set; } = false;
}

public enum ContentStatus
{
    Draft = 0,
    PendingReview = 1,
    Approved = 2,
    Published = 3,
    Rejected = 4,
    Archived = 5
}
