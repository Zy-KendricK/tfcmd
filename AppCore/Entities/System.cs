using System.ComponentModel.DataAnnotations;

namespace AppCore.Entities;

/// <summary>
/// System settings (key-value store)
/// </summary>
public class Setting : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Key { get; set; } = string.Empty;

    public string? Value { get; set; }

    [MaxLength(100)]
    public string? Group { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public SettingValueType ValueType { get; set; } = SettingValueType.String;

    public bool IsPublic { get; set; } = false;

    public bool IsEditable { get; set; } = true;
}

public enum SettingValueType
{
    String = 0,
    Number = 1,
    Boolean = 2,
    Json = 3,
    Html = 4
}

/// <summary>
/// Notification
/// </summary>
public class Notification : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public virtual ApplicationUser User { get; set; } = null!;

    [Required]
    [MaxLength(500)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Message { get; set; }

    public NotificationType Type { get; set; } = NotificationType.Info;

    [MaxLength(500)]
    public string? Link { get; set; }

    [MaxLength(100)]
    public string? Icon { get; set; }

    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }

    public string? SenderId { get; set; }
    public virtual ApplicationUser? Sender { get; set; }

    /// <summary>
    /// Related entity type (Post, Job, etc.)
    /// </summary>
    [MaxLength(50)]
    public string? EntityType { get; set; }

    public int? EntityId { get; set; }
}

public enum NotificationType
{
    Info = 0,
    Success = 1,
    Warning = 2,
    Error = 3,
    NewMessage = 4,
    NewFollower = 5,
    NewComment = 6,
    NewLike = 7,
    Mention = 8,
    System = 9
}

/// <summary>
/// Private message
/// </summary>
public class Message : BaseEntity
{
    public string SenderId { get; set; } = string.Empty;
    public virtual ApplicationUser Sender { get; set; } = null!;

    public string RecipientId { get; set; } = string.Empty;
    public virtual ApplicationUser Recipient { get; set; } = null!;

    [Required]
    public string Content { get; set; } = string.Empty;

    public int? ConversationId { get; set; }
    public virtual Conversation? Conversation { get; set; }

    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }

    public bool SenderDeleted { get; set; } = false;
    public bool RecipientDeleted { get; set; } = false;

    public virtual ICollection<MessageAttachment> Attachments { get; set; } = new List<MessageAttachment>();
}

/// <summary>
/// Message conversation
/// </summary>
public class Conversation : BaseEntity
{
    [MaxLength(200)]
    public string? Subject { get; set; }

    public bool IsGroupConversation { get; set; } = false;

    public DateTime? LastMessageAt { get; set; }

    public int MessageCount { get; set; } = 0;

    public virtual ICollection<ConversationParticipant> Participants { get; set; } = new List<ConversationParticipant>();
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}

/// <summary>
/// Conversation participant
/// </summary>
public class ConversationParticipant : BaseEntity
{
    public int ConversationId { get; set; }
    public virtual Conversation Conversation { get; set; } = null!;

    public string UserId { get; set; } = string.Empty;
    public virtual ApplicationUser User { get; set; } = null!;

    public int UnreadCount { get; set; } = 0;
    public DateTime? LastReadAt { get; set; }

    public bool IsArchived { get; set; } = false;
    public bool IsMuted { get; set; } = false;
}

/// <summary>
/// Message attachment
/// </summary>
public class MessageAttachment : BaseEntity
{
    public int MessageId { get; set; }
    public virtual Message Message { get; set; } = null!;

    [Required]
    [MaxLength(200)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Url { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? MimeType { get; set; }

    public long? FileSize { get; set; }
}

/// <summary>
/// Audit log
/// </summary>
public class AuditLog : BaseEntity
{
    public string? UserId { get; set; }
    public virtual ApplicationUser? User { get; set; }

    [Required]
    [MaxLength(100)]
    public string Action { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? EntityType { get; set; }

    public int? EntityId { get; set; }

    public string? OldValues { get; set; }
    public string? NewValues { get; set; }

    [MaxLength(100)]
    public string? IpAddress { get; set; }

    [MaxLength(500)]
    public string? UserAgent { get; set; }
}

/// <summary>
/// Media file (uploaded files)
/// </summary>
public class MediaFile : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Url { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? ThumbnailUrl { get; set; }

    [MaxLength(100)]
    public string MimeType { get; set; } = string.Empty;

    public long FileSize { get; set; }

    public MediaFileType Type { get; set; } = MediaFileType.Other;

    public string? UploadedById { get; set; }
    public virtual ApplicationUser? UploadedBy { get; set; }

    public int? FolderId { get; set; }
    public virtual MediaFolder? Folder { get; set; }

    [MaxLength(200)]
    public string? AltText { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public int? Width { get; set; }
    public int? Height { get; set; }
    public int? Duration { get; set; }
}

public enum MediaFileType
{
    Image = 0,
    Video = 1,
    Audio = 2,
    Document = 3,
    Archive = 4,
    Other = 5
}

/// <summary>
/// Media folder
/// </summary>
public class MediaFolder : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public int? ParentFolderId { get; set; }
    public virtual MediaFolder? ParentFolder { get; set; }

    public string? OwnerId { get; set; }
    public virtual ApplicationUser? Owner { get; set; }

    public virtual ICollection<MediaFolder> ChildFolders { get; set; } = new List<MediaFolder>();
    public virtual ICollection<MediaFile> Files { get; set; } = new List<MediaFile>();
}

/// <summary>
/// Page (static pages)
/// </summary>
public class Page : PublishableEntity
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Slug { get; set; } = string.Empty;

    public string? Content { get; set; }

    [MaxLength(500)]
    public string? MetaTitle { get; set; }

    [MaxLength(500)]
    public string? MetaDescription { get; set; }

    [MaxLength(500)]
    public string? MetaKeywords { get; set; }

    [MaxLength(500)]
    public string? FeaturedImageUrl { get; set; }

    public int? ParentPageId { get; set; }
    public virtual Page? ParentPage { get; set; }

    [MaxLength(100)]
    public string? Template { get; set; }

    public int DisplayOrder { get; set; } = 0;
    public bool ShowInNavigation { get; set; } = true;

    public virtual ICollection<Page> ChildPages { get; set; } = new List<Page>();
}

/// <summary>
/// Menu
/// </summary>
public class Menu : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Slug { get; set; }

    [MaxLength(100)]
    public string? Location { get; set; }

    public virtual ICollection<MenuItem> Items { get; set; } = new List<MenuItem>();
}

/// <summary>
/// Menu item
/// </summary>
public class MenuItem : BaseEntity
{
    public int MenuId { get; set; }
    public virtual Menu Menu { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Url { get; set; }

    public int? PageId { get; set; }
    public virtual Page? Page { get; set; }

    public int? ParentItemId { get; set; }
    public virtual MenuItem? ParentItem { get; set; }

    [MaxLength(100)]
    public string? Icon { get; set; }

    [MaxLength(100)]
    public string? CssClass { get; set; }

    [MaxLength(50)]
    public string? Target { get; set; }

    public int DisplayOrder { get; set; } = 0;

    public virtual ICollection<MenuItem> ChildItems { get; set; } = new List<MenuItem>();
}
