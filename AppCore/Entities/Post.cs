using System.ComponentModel.DataAnnotations;

namespace AppCore.Entities;

/// <summary>
/// Blog post or article
/// </summary>
public class Post : PublishableEntity
{
    [Required]
    [MaxLength(300)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Slug { get; set; }

    [MaxLength(500)]
    public string? Excerpt { get; set; }

    public string? Content { get; set; }

    [MaxLength(500)]
    public string? FeaturedImageUrl { get; set; }

    public string AuthorId { get; set; } = string.Empty;
    public virtual ApplicationUser Author { get; set; } = null!;

    public int? CategoryId { get; set; }
    public virtual Category? Category { get; set; }

    /// <summary>
    /// Post format: Standard, Gallery, Video, Audio, Link
    /// </summary>
    public PostFormat Format { get; set; } = PostFormat.Standard;

    [MaxLength(500)]
    public string? VideoUrl { get; set; }

    [MaxLength(500)]
    public string? AudioUrl { get; set; }

    [MaxLength(500)]
    public string? LinkUrl { get; set; }

    public int ViewCount { get; set; } = 0;
    public int LikeCount { get; set; } = 0;
    public int CommentCount { get; set; } = 0;

    /// <summary>
    /// Whether comments are allowed
    /// </summary>
    public bool AllowComments { get; set; } = true;

    /// <summary>
    /// Whether this post is featured
    /// </summary>
    public bool IsFeatured { get; set; } = false;

    /// <summary>
    /// SEO meta title
    /// </summary>
    [MaxLength(100)]
    public string? MetaTitle { get; set; }

    /// <summary>
    /// SEO meta description
    /// </summary>
    [MaxLength(300)]
    public string? MetaDescription { get; set; }

    // Navigation properties
    public virtual ICollection<PostComment> Comments { get; set; } = new List<PostComment>();
    public virtual ICollection<PostTag> Tags { get; set; } = new List<PostTag>();
    public virtual ICollection<PostImage> Images { get; set; } = new List<PostImage>();
}

public enum PostFormat
{
    Standard = 0,
    Gallery = 1,
    Video = 2,
    Audio = 3,
    Link = 4,
    Quote = 5
}

/// <summary>
/// Comment on a post
/// </summary>
public class PostComment : BaseEntity
{
    public int PostId { get; set; }
    public virtual Post Post { get; set; } = null!;

    public string UserId { get; set; } = string.Empty;
    public virtual ApplicationUser User { get; set; } = null!;

    [Required]
    [MaxLength(2000)]
    public string Content { get; set; } = string.Empty;

    public int? ParentCommentId { get; set; }
    public virtual PostComment? ParentComment { get; set; }

    public bool IsApproved { get; set; } = true;

    public virtual ICollection<PostComment> Replies { get; set; } = new List<PostComment>();
}

/// <summary>
/// Tag
/// </summary>
public class Tag : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Slug { get; set; }

    [MaxLength(300)]
    public string? Description { get; set; }

    public virtual ICollection<PostTag> Posts { get; set; } = new List<PostTag>();
}

/// <summary>
/// Post-Tag relationship
/// </summary>
public class PostTag
{
    public int PostId { get; set; }
    public virtual Post Post { get; set; } = null!;

    public int TagId { get; set; }
    public virtual Tag Tag { get; set; } = null!;
}

/// <summary>
/// Gallery image for posts
/// </summary>
public class PostImage
{
    public int Id { get; set; }

    public int PostId { get; set; }
    public virtual Post Post { get; set; } = null!;

    [Required]
    [MaxLength(500)]
    public string ImageUrl { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Caption { get; set; }

    public int DisplayOrder { get; set; } = 0;
}
