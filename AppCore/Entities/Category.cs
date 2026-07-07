using System.ComponentModel.DataAnnotations;

namespace AppCore.Entities;

/// <summary>
/// Generic category entity that can be used for posts, groups, etc.
/// </summary>
public class Category : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Slug { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(500)]
    public string? IconUrl { get; set; }

    [MaxLength(50)]
    public string? Color { get; set; }

    /// <summary>
    /// Category type to differentiate between post categories, group categories, etc.
    /// </summary>
    [MaxLength(50)]
    public string CategoryType { get; set; } = "General";

    /// <summary>
    /// Parent category for hierarchical structure
    /// </summary>
    public int? ParentId { get; set; }
    public virtual Category? Parent { get; set; }

    /// <summary>
    /// Display order
    /// </summary>
    public int SortOrder { get; set; } = 0;

    /// <summary>
    /// Count of items in this category
    /// </summary>
    public int ItemCount { get; set; } = 0;

    // Navigation properties
    public virtual ICollection<Category> Children { get; set; } = new List<Category>();
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    public virtual ICollection<SocialGroup> Groups { get; set; } = new List<SocialGroup>();
}
