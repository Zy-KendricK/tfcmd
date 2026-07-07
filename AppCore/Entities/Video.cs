using System.ComponentModel.DataAnnotations;

namespace AppCore.Entities;

/// <summary>
/// Video content
/// </summary>
public class Video : PublishableEntity
{
    [Required]
    [MaxLength(300)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Slug { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(500)]
    public string Url { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? ThumbnailUrl { get; set; }

    [MaxLength(500)]
    public string? PreviewUrl { get; set; }

    /// <summary>
    /// Video source type
    /// </summary>
    public VideoSource Source { get; set; } = VideoSource.Upload;

    /// <summary>
    /// External video ID (YouTube, Vimeo, etc.)
    /// </summary>
    [MaxLength(100)]
    public string? ExternalId { get; set; }

    /// <summary>
    /// Embed URL for external videos
    /// </summary>
    [MaxLength(500)]
    public string? EmbedUrl { get; set; }

    public int? CategoryId { get; set; }
    public virtual VideoCategory? Category { get; set; }

    public string UploadedById { get; set; } = string.Empty;
    public virtual ApplicationUser UploadedBy { get; set; } = null!;

    /// <summary>
    /// Duration in seconds
    /// </summary>
    public int? Duration { get; set; }

    /// <summary>
    /// Width in pixels
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// Height in pixels
    /// </summary>
    public int? Height { get; set; }

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long? FileSize { get; set; }

    [MaxLength(50)]
    public string? MimeType { get; set; }

    [MaxLength(50)]
    public string? Resolution { get; set; }

    public VideoPrivacy Privacy { get; set; } = VideoPrivacy.Public;

    public bool IsFeatured { get; set; } = false;

    public int ViewCount { get; set; } = 0;
    public int LikeCount { get; set; } = 0;
    public int DislikeCount { get; set; } = 0;
    public int CommentCount { get; set; } = 0;
    public int ShareCount { get; set; } = 0;

    public bool AllowComments { get; set; } = true;
    public bool AllowDownload { get; set; } = false;
    public bool AllowEmbed { get; set; } = true;

    /// <summary>
    /// Age restriction
    /// </summary>
    public bool IsAgeRestricted { get; set; } = false;

    // Navigation properties
    public virtual ICollection<VideoComment> Comments { get; set; } = new List<VideoComment>();
    public virtual ICollection<VideoLike> Likes { get; set; } = new List<VideoLike>();
    public virtual ICollection<VideoPlaylist> Playlists { get; set; } = new List<VideoPlaylist>();
}

public enum VideoSource
{
    Upload = 0,
    YouTube = 1,
    Vimeo = 2,
    Dailymotion = 3,
    External = 4
}

public enum VideoPrivacy
{
    Public = 0,
    Unlisted = 1,
    Private = 2,
    Friends = 3
}

/// <summary>
/// Video category
/// </summary>
public class VideoCategory : BaseEntity
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
    public virtual VideoCategory? ParentCategory { get; set; }

    public int DisplayOrder { get; set; } = 0;

    public virtual ICollection<VideoCategory> ChildCategories { get; set; } = new List<VideoCategory>();
    public virtual ICollection<Video> Videos { get; set; } = new List<Video>();
}

/// <summary>
/// Video comment
/// </summary>
public class VideoComment : BaseEntity
{
    public int VideoId { get; set; }
    public virtual Video Video { get; set; } = null!;

    public string CommenterId { get; set; } = string.Empty;
    public virtual ApplicationUser Commenter { get; set; } = null!;

    [Required]
    [MaxLength(2000)]
    public string Content { get; set; } = string.Empty;

    public int? ParentCommentId { get; set; }
    public virtual VideoComment? ParentComment { get; set; }

    public int LikeCount { get; set; } = 0;
    public int DislikeCount { get; set; } = 0;

    /// <summary>
    /// Timestamp in video (seconds)
    /// </summary>
    public int? Timestamp { get; set; }

    public bool IsPinned { get; set; } = false;

    public virtual ICollection<VideoComment> Replies { get; set; } = new List<VideoComment>();
}

/// <summary>
/// Video like/dislike
/// </summary>
public class VideoLike : BaseEntity
{
    public int VideoId { get; set; }
    public virtual Video Video { get; set; } = null!;

    public string UserId { get; set; } = string.Empty;
    public virtual ApplicationUser User { get; set; } = null!;

    public bool IsLike { get; set; } = true;
}

/// <summary>
/// Video playlist
/// </summary>
public class Playlist : PublishableEntity
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

    [MaxLength(500)]
    public string? ThumbnailUrl { get; set; }

    public VideoPrivacy Privacy { get; set; } = VideoPrivacy.Public;

    public int VideoCount { get; set; } = 0;
    public int ViewCount { get; set; } = 0;

    public virtual ICollection<VideoPlaylist> Videos { get; set; } = new List<VideoPlaylist>();
}

/// <summary>
/// Video-Playlist relationship
/// </summary>
public class VideoPlaylist : BaseEntity
{
    public int VideoId { get; set; }
    public virtual Video Video { get; set; } = null!;

    public int PlaylistId { get; set; }
    public virtual Playlist Playlist { get; set; } = null!;

    public int DisplayOrder { get; set; } = 0;
}
