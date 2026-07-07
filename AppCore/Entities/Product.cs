using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppCore.Entities;

/// <summary>
/// Product for shop/marketplace
/// </summary>
public class Product : PublishableEntity
{
    [Required]
    [MaxLength(300)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Slug { get; set; }

    [MaxLength(100)]
    public string? SKU { get; set; }

    [MaxLength(500)]
    public string? ShortDescription { get; set; }

    public string? Description { get; set; }

    public int? CategoryId { get; set; }
    public virtual ProductCategory? Category { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? CompareAtPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? CostPrice { get; set; }

    [MaxLength(10)]
    public string Currency { get; set; } = "USD";

    public ProductType Type { get; set; } = ProductType.Physical;

    /// <summary>
    /// Stock quantity
    /// </summary>
    public int? StockQuantity { get; set; }

    public bool TrackInventory { get; set; } = true;
    public bool AllowBackorder { get; set; } = false;

    /// <summary>
    /// Weight in grams
    /// </summary>
    public double? Weight { get; set; }

    /// <summary>
    /// Dimensions in cm
    /// </summary>
    public double? Length { get; set; }
    public double? Width { get; set; }
    public double? Height { get; set; }

    public string SellerId { get; set; } = string.Empty;
    public virtual ApplicationUser Seller { get; set; } = null!;

    public bool IsFeatured { get; set; } = false;
    public bool IsOnSale { get; set; } = false;
    public bool IsBestseller { get; set; } = false;
    public bool IsNewArrival { get; set; } = false;

    public int ViewCount { get; set; } = 0;
    public int SoldCount { get; set; } = 0;

    /// <summary>
    /// Average rating (1-5)
    /// </summary>
    [Column(TypeName = "decimal(3,2)")]
    public decimal? AverageRating { get; set; }

    public int ReviewCount { get; set; } = 0;

    /// <summary>
    /// For digital products: download URL
    /// </summary>
    [MaxLength(500)]
    public string? DownloadUrl { get; set; }

    public int? DownloadLimit { get; set; }
    public int? DownloadExpiry { get; set; }

    // Navigation properties
    public virtual ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public virtual ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    public virtual ICollection<ProductReview> Reviews { get; set; } = new List<ProductReview>();
    public virtual ICollection<ProductTag> Tags { get; set; } = new List<ProductTag>();
}

public enum ProductType
{
    Physical = 0,
    Digital = 1,
    Service = 2,
    Subscription = 3
}

/// <summary>
/// Product category
/// </summary>
public class ProductCategory : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Slug { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    [MaxLength(100)]
    public string? Icon { get; set; }

    public int? ParentCategoryId { get; set; }
    public virtual ProductCategory? ParentCategory { get; set; }

    public int DisplayOrder { get; set; } = 0;
    public bool IsFeatured { get; set; } = false;

    public virtual ICollection<ProductCategory> ChildCategories { get; set; } = new List<ProductCategory>();
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}

/// <summary>
/// Product image
/// </summary>
public class ProductImage : BaseEntity
{
    public int ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;

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
/// Product variant (size, color, etc.)
/// </summary>
public class ProductVariant : BaseEntity
{
    public int ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? SKU { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? Price { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? CompareAtPrice { get; set; }

    public int? StockQuantity { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Variant options as JSON
    /// </summary>
    [MaxLength(1000)]
    public string? Options { get; set; }
}

/// <summary>
/// Product review
/// </summary>
public class ProductReview : BaseEntity
{
    public int ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;

    public string ReviewerId { get; set; } = string.Empty;
    public virtual ApplicationUser Reviewer { get; set; } = null!;

    /// <summary>
    /// Rating 1-5
    /// </summary>
    [Range(1, 5)]
    public int Rating { get; set; }

    [MaxLength(200)]
    public string? Title { get; set; }

    [MaxLength(2000)]
    public string? Comment { get; set; }

    public bool IsVerifiedPurchase { get; set; } = false;
    public bool IsApproved { get; set; } = false;

    public int HelpfulCount { get; set; } = 0;
}

/// <summary>
/// Product tag
/// </summary>
public class ProductTag
{
    public int ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;

    public int TagId { get; set; }
    public virtual Tag Tag { get; set; } = null!;
}
