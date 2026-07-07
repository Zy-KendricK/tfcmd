using System.ComponentModel.DataAnnotations;

namespace AppCore.Entities;

/// <summary>
/// Photo in gallery
/// </summary>
public class Photo : PublishableEntity
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Slug { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(500)]
    public string Url { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? ThumbnailUrl { get; set; }

    [MaxLength(500)]
    public string? MediumUrl { get; set; }

    [MaxLength(500)]
    public string? OriginalUrl { get; set; }

    public int? AlbumId { get; set; }
    public virtual PhotoAlbum? Album { get; set; }

    public string UploadedById { get; set; } = string.Empty;
    public virtual ApplicationUser UploadedBy { get; set; } = null!;

    /// <summary>
    /// Image width in pixels
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// Image height in pixels
    /// </summary>
    public int? Height { get; set; }

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long? FileSize { get; set; }

    [MaxLength(50)]
    public string? MimeType { get; set; }

    /// <summary>
    /// Camera/device info
    /// </summary>
    [MaxLength(200)]
    public string? Camera { get; set; }

    /// <summary>
    /// Location where photo was taken
    /// </summary>
    [MaxLength(200)]
    public string? Location { get; set; }

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    /// <summary>
    /// Date photo was taken
    /// </summary>
    public DateTime? TakenAt { get; set; }

    public PhotoPrivacy Privacy { get; set; } = PhotoPrivacy.Public;

    public bool IsFeatured { get; set; } = false;

    public int ViewCount { get; set; } = 0;
    public int LikeCount { get; set; } = 0;
    public int DownloadCount { get; set; } = 0;

    public bool AllowDownload { get; set; } = true;
    public bool AllowComments { get; set; } = true;

    // Navigation properties
    public virtual ICollection<PhotoComment> Comments { get; set; } = new List<PhotoComment>();
    public virtual ICollection<PhotoLike> Likes { get; set; } = new List<PhotoLike>();
    public virtual ICollection<PhotoTag> Tags { get; set; } = new List<PhotoTag>();
}

public enum PhotoPrivacy
{
    Public = 0,
    Friends = 1,
    Private = 2,
    Album = 3 // Inherits from album
}

/// <summary>
/// Photo album
/// </summary>
public class PhotoAlbum : PublishableEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Slug { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    public string OwnerId { get; set; } = string.Empty;
    public virtual ApplicationUser Owner { get; set; } = null!;

    /// <summary>
    /// Cover photo ID
    /// </summary>
    public int? CoverPhotoId { get; set; }

    [MaxLength(500)]
    public string? CoverUrl { get; set; }

    public PhotoPrivacy Privacy { get; set; } = PhotoPrivacy.Public;

    public int PhotoCount { get; set; } = 0;
    public int ViewCount { get; set; } = 0;

    public bool AllowContributions { get; set; } = false;

    public virtual ICollection<Photo> Photos { get; set; } = new List<Photo>();
}

/// <summary>
/// Photo comment
/// </summary>
public class PhotoComment : BaseEntity
{
    public int PhotoId { get; set; }
    public virtual Photo Photo { get; set; } = null!;

    public string CommenterId { get; set; } = string.Empty;
    public virtual ApplicationUser Commenter { get; set; } = null!;

    [Required]
    [MaxLength(1000)]
    public string Content { get; set; } = string.Empty;

    public int? ParentCommentId { get; set; }
    public virtual PhotoComment? ParentComment { get; set; }

    public int LikeCount { get; set; } = 0;

    public virtual ICollection<PhotoComment> Replies { get; set; } = new List<PhotoComment>();
}

/// <summary>
/// Photo like
/// </summary>
public class PhotoLike : BaseEntity
{
    public int PhotoId { get; set; }
    public virtual Photo Photo { get; set; } = null!;

    public string UserId { get; set; } = string.Empty;
    public virtual ApplicationUser User { get; set; } = null!;
}

/// <summary>
/// Photo tag (person tag)
/// </summary>
public class PhotoTag : BaseEntity
{
    public int PhotoId { get; set; }
    public virtual Photo Photo { get; set; } = null!;

    public string? TaggedUserId { get; set; }
    public virtual ApplicationUser? TaggedUser { get; set; }

    [MaxLength(100)]
    public string? TagText { get; set; }

    /// <summary>
    /// X position (percentage)
    /// </summary>
    public double? PositionX { get; set; }

    /// <summary>
    /// Y position (percentage)
    /// </summary>
    public double? PositionY { get; set; }
}
