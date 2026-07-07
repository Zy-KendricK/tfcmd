using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppCore.Entities;

/// <summary>
/// Classified advertisement
/// </summary>
public class Advert : PublishableEntity
{
    [Required]
    [MaxLength(300)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Slug { get; set; }

    [Required]
    public string Description { get; set; } = string.Empty;

    public int? CategoryId { get; set; }
    public virtual AdvertCategory? Category { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? Price { get; set; }

    [MaxLength(10)]
    public string? Currency { get; set; } = "USD";

    public PriceType PriceType { get; set; } = PriceType.Fixed;

    public bool IsNegotiable { get; set; } = false;

    public AdvertCondition Condition { get; set; } = AdvertCondition.New;

    [MaxLength(200)]
    public string? Location { get; set; }

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public string PostedById { get; set; } = string.Empty;
    public virtual ApplicationUser PostedBy { get; set; } = null!;

    [MaxLength(255)]
    public string? ContactEmail { get; set; }

    [MaxLength(50)]
    public string? ContactPhone { get; set; }

    public bool ShowContactInfo { get; set; } = true;

    public bool IsFeatured { get; set; } = false;
    public bool IsUrgent { get; set; } = false;

    public int ViewCount { get; set; } = 0;
    public int InquiryCount { get; set; } = 0;

    /// <summary>
    /// When the advert expires
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    // Navigation properties
    public virtual ICollection<AdvertImage> Images { get; set; } = new List<AdvertImage>();
    public virtual ICollection<AdvertInquiry> Inquiries { get; set; } = new List<AdvertInquiry>();
    public virtual ICollection<AdvertFavorite> Favorites { get; set; } = new List<AdvertFavorite>();
}

public enum PriceType
{
    Fixed = 0,
    Negotiable = 1,
    Free = 2,
    ContactForPrice = 3,
    Auction = 4
}

public enum AdvertCondition
{
    New = 0,
    LikeNew = 1,
    Good = 2,
    Fair = 3,
    Poor = 4,
    ForParts = 5
}

/// <summary>
/// Advert category
/// </summary>
public class AdvertCategory : BaseEntity
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

    public int? ParentCategoryId { get; set; }
    public virtual AdvertCategory? ParentCategory { get; set; }

    public int DisplayOrder { get; set; } = 0;

    public virtual ICollection<AdvertCategory> ChildCategories { get; set; } = new List<AdvertCategory>();
    public virtual ICollection<Advert> Adverts { get; set; } = new List<Advert>();
}

/// <summary>
/// Advert image
/// </summary>
public class AdvertImage : BaseEntity
{
    public int AdvertId { get; set; }
    public virtual Advert Advert { get; set; } = null!;

    [Required]
    [MaxLength(500)]
    public string Url { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? ThumbnailUrl { get; set; }

    [MaxLength(200)]
    public string? AltText { get; set; }

    public int DisplayOrder { get; set; } = 0;
    public bool IsPrimary { get; set; } = false;
}

/// <summary>
/// Advert inquiry/message
/// </summary>
public class AdvertInquiry : BaseEntity
{
    public int AdvertId { get; set; }
    public virtual Advert Advert { get; set; } = null!;

    public string? InquirerId { get; set; }
    public virtual ApplicationUser? Inquirer { get; set; }

    [MaxLength(100)]
    public string? GuestName { get; set; }

    [MaxLength(255)]
    public string? GuestEmail { get; set; }

    [MaxLength(50)]
    public string? GuestPhone { get; set; }

    [Required]
    [MaxLength(2000)]
    public string Message { get; set; } = string.Empty;

    public bool IsRead { get; set; } = false;
    public bool IsReplied { get; set; } = false;

    [MaxLength(2000)]
    public string? Reply { get; set; }

    public DateTime? RepliedAt { get; set; }
}

/// <summary>
/// Favorite adverts
/// </summary>
public class AdvertFavorite : BaseEntity
{
    public int AdvertId { get; set; }
    public virtual Advert Advert { get; set; } = null!;

    public string UserId { get; set; } = string.Empty;
    public virtual ApplicationUser User { get; set; } = null!;
}
