using System.ComponentModel.DataAnnotations;

namespace AppCore.Entities;

/// <summary>
/// Frequently asked questions
/// </summary>
public class Faq : PublishableEntity
{
    [Required]
    [MaxLength(500)]
    public string Question { get; set; } = string.Empty;

    [Required]
    public string Answer { get; set; } = string.Empty;

    public int? CategoryId { get; set; }
    public virtual FaqCategory? Category { get; set; }

    public int DisplayOrder { get; set; } = 0;

    public int ViewCount { get; set; } = 0;
    public int HelpfulCount { get; set; } = 0;
    public int NotHelpfulCount { get; set; } = 0;
}

/// <summary>
/// FAQ category
/// </summary>
public class FaqCategory : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Slug { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(100)]
    public string? Icon { get; set; }

    public int DisplayOrder { get; set; } = 0;

    public virtual ICollection<Faq> Faqs { get; set; } = new List<Faq>();
}

/// <summary>
/// Contact form submission
/// </summary>
public class ContactMessage : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Phone { get; set; }

    [MaxLength(200)]
    public string? Subject { get; set; }

    public int? CategoryId { get; set; }
    public virtual ContactCategory? Category { get; set; }

    [Required]
    public string Message { get; set; } = string.Empty;

    public string? UserId { get; set; }
    public virtual ApplicationUser? User { get; set; }

    public ContactMessageStatus Status { get; set; } = ContactMessageStatus.New;

    public string? AssignedToId { get; set; }
    public virtual ApplicationUser? AssignedTo { get; set; }

    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }

    public string? Notes { get; set; }

    [MaxLength(100)]
    public string? IpAddress { get; set; }

    public virtual ICollection<ContactMessageReply> Replies { get; set; } = new List<ContactMessageReply>();
}

public enum ContactMessageStatus
{
    New = 0,
    InProgress = 1,
    Resolved = 2,
    Closed = 3,
    Spam = 4
}

/// <summary>
/// Contact message category
/// </summary>
public class ContactCategory : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(255)]
    public string? EmailRecipient { get; set; }

    public int DisplayOrder { get; set; } = 0;

    public virtual ICollection<ContactMessage> Messages { get; set; } = new List<ContactMessage>();
}

/// <summary>
/// Contact message reply
/// </summary>
public class ContactMessageReply : BaseEntity
{
    public int MessageId { get; set; }
    public virtual ContactMessage ContactMessage { get; set; } = null!;

    public string ReplierId { get; set; } = string.Empty;
    public virtual ApplicationUser Replier { get; set; } = null!;

    [Required]
    public string Content { get; set; } = string.Empty;

    public bool EmailSent { get; set; } = false;
    public DateTime? EmailSentAt { get; set; }
}

/// <summary>
/// About page content
/// </summary>
public class AboutPage : PublishableEntity
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Slug { get; set; }

    [MaxLength(500)]
    public string? Subtitle { get; set; }

    public string? Content { get; set; }

    [MaxLength(500)]
    public string? FeaturedImageUrl { get; set; }

    public int DisplayOrder { get; set; } = 0;

    public AboutPageSection Section { get; set; } = AboutPageSection.Main;
}

public enum AboutPageSection
{
    Main = 0,
    Mission = 1,
    Vision = 2,
    History = 3,
    Values = 4,
    Team = 5,
    Testimonials = 6,
    Partners = 7
}

/// <summary>
/// Team member
/// </summary>
public class TeamMember : PublishableEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Title { get; set; }

    [MaxLength(100)]
    public string? Department { get; set; }

    [MaxLength(1000)]
    public string? Bio { get; set; }

    [MaxLength(500)]
    public string? PhotoUrl { get; set; }

    [MaxLength(255)]
    public string? Email { get; set; }

    [MaxLength(50)]
    public string? Phone { get; set; }

    [MaxLength(500)]
    public string? LinkedInUrl { get; set; }

    [MaxLength(500)]
    public string? TwitterUrl { get; set; }

    public int DisplayOrder { get; set; } = 0;
    public bool IsLeadership { get; set; } = false;
}

/// <summary>
/// Charity/cause content
/// </summary>
public class Charity : PublishableEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Slug { get; set; }

    [MaxLength(500)]
    public string? Tagline { get; set; }

    public string? Description { get; set; }

    [MaxLength(500)]
    public string? LogoUrl { get; set; }

    [MaxLength(500)]
    public string? CoverImageUrl { get; set; }

    [MaxLength(255)]
    public string? Website { get; set; }

    [MaxLength(255)]
    public string? Email { get; set; }

    [MaxLength(50)]
    public string? Phone { get; set; }

    public string? Address { get; set; }

    public bool IsFeatured { get; set; } = false;

    public virtual ICollection<CharityProject> Projects { get; set; } = new List<CharityProject>();
}

/// <summary>
/// Charity project/campaign
/// </summary>
public class CharityProject : PublishableEntity
{
    public int CharityId { get; set; }
    public virtual Charity Charity { get; set; } = null!;

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Slug { get; set; }

    public string? Description { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    public decimal? GoalAmount { get; set; }
    public decimal? CurrentAmount { get; set; }

    [MaxLength(10)]
    public string? Currency { get; set; } = "USD";

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public int DonorCount { get; set; } = 0;

    public bool IsFeatured { get; set; } = false;
}

/// <summary>
/// Sections of the website charity page, in the order they appear on the page.
/// </summary>
public enum CharityPageSection
{
    Banner = 1,
    Feature = 2,
    History = 3,
    Mission = 4,
    Counter = 5,
    Event = 6,
    Partner = 7
}

/// <summary>
/// A single admin-managed content item belonging to a website charity page section.
/// Generic fields are reused per section (e.g. Number for counters, Date for events).
/// </summary>
public class CharityPageItem : PublishableEntity
{
    public CharityPageSection Section { get; set; }

    [MaxLength(200)]
    public string? Title { get; set; }

    /// <summary>Optional highlighted (accent-colored) part of the title.</summary>
    [MaxLength(200)]
    public string? Highlight { get; set; }

    public string? Text { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    [MaxLength(500)]
    public string? LinkUrl { get; set; }

    /// <summary>Extra metadata, e.g. comma-separated banner badges.</summary>
    [MaxLength(300)]
    public string? Meta { get; set; }

    /// <summary>Numeric value, e.g. counter total.</summary>
    public int? Number { get; set; }

    /// <summary>Date value, e.g. event date.</summary>
    public DateTime? Date { get; set; }

    public int DisplayOrder { get; set; } = 0;
}
