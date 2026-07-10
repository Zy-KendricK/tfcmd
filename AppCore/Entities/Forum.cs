using System.ComponentModel.DataAnnotations;

namespace AppCore.Entities;

/// <summary>
/// Forum for discussions
/// </summary>
public class Forum : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Slug { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    [MaxLength(500)]
    public string? IconUrl { get; set; }

    [MaxLength(100)]
    public string? Icon { get; set; }

    public int? ParentForumId { get; set; }
    public virtual Forum? ParentForum { get; set; }

    public int DisplayOrder { get; set; } = 0;

    public ForumVisibility Visibility { get; set; } = ForumVisibility.Public;

    /// <summary>
    /// Who can create topics
    /// </summary>
    public ForumPermissionLevel TopicPermission { get; set; } = ForumPermissionLevel.RegisteredUsers;

    /// <summary>
    /// Who can reply to topics
    /// </summary>
    public ForumPermissionLevel ReplyPermission { get; set; } = ForumPermissionLevel.RegisteredUsers;

    public bool IsClosed { get; set; } = false;

    public int TopicCount { get; set; } = 0;
    public int PostCount { get; set; } = 0;

    public int? LastPostId { get; set; }
    public DateTime? LastActivityAt { get; set; }

    public virtual ICollection<Forum> ChildForums { get; set; } = new List<Forum>();
    public virtual ICollection<ForumTopic> Topics { get; set; } = new List<ForumTopic>();
    public virtual ICollection<ForumModerator> Moderators { get; set; } = new List<ForumModerator>();
}

public enum ForumVisibility
{
    Public = 0,
    RegisteredOnly = 1,
    Private = 2,
    Hidden = 3
}

public enum ForumPermissionLevel
{
    Everyone = 0,
    RegisteredUsers = 1,
    Moderators = 2,
    Admins = 3
}

/// <summary>
/// Forum topic/thread
/// </summary>
public class ForumTopic : PublishableEntity
{
    [Required]
    [MaxLength(300)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? Slug { get; set; }

    public int ForumId { get; set; }
    public virtual Forum Forum { get; set; } = null!;

    public string AuthorId { get; set; } = string.Empty;
    public virtual ApplicationUser Author { get; set; } = null!;

    [Required]
    public string Content { get; set; } = string.Empty;

    public TopicType Type { get; set; } = TopicType.Discussion;

    public bool IsSticky { get; set; } = false;
    public bool IsLocked { get; set; } = false;
    public bool IsAnnouncement { get; set; } = false;

    public int ViewCount { get; set; } = 0;
    public int ReplyCount { get; set; } = 0;
    public int LikeCount { get; set; } = 0;

    public int? LastPostId { get; set; }
    public virtual ForumPost? LastPost { get; set; }

    public DateTime? LastActivityAt { get; set; }

    public virtual ICollection<ForumPost> Posts { get; set; } = new List<ForumPost>();
    public virtual ICollection<ForumTopicTag> Tags { get; set; } = new List<ForumTopicTag>();
    public virtual ICollection<ForumTopicSubscriber> Subscribers { get; set; } = new List<ForumTopicSubscriber>();
}

public enum TopicType
{
    Discussion = 0,
    Question = 1,
    Poll = 2,
    Announcement = 3
}

/// <summary>
/// Forum post/reply
/// </summary>
public class ForumPost : BaseEntity
{
    public int TopicId { get; set; }
    public virtual ForumTopic Topic { get; set; } = null!;

    public string AuthorId { get; set; } = string.Empty;
    public virtual ApplicationUser Author { get; set; } = null!;

    [Required]
    public string Content { get; set; } = string.Empty;

    public int? ParentPostId { get; set; }
    public virtual ForumPost? ParentPost { get; set; }

    public int LikeCount { get; set; } = 0;

    public bool IsEdited { get; set; } = false;
    public DateTime? EditedAt { get; set; }

    [MaxLength(200)]
    public string? EditReason { get; set; }

    /// <summary>
    /// Marked as best answer (for question topics)
    /// </summary>
    public bool IsBestAnswer { get; set; } = false;

    public virtual ICollection<ForumPost> Replies { get; set; } = new List<ForumPost>();
    public virtual ICollection<ForumPostLike> Likes { get; set; } = new List<ForumPostLike>();
}

/// <summary>
/// Forum post like
/// </summary>
public class ForumPostLike : BaseEntity
{
    public int PostId { get; set; }
    public virtual ForumPost Post { get; set; } = null!;

    public string UserId { get; set; } = string.Empty;
    public virtual ApplicationUser User { get; set; } = null!;
}

/// <summary>
/// Forum moderator
/// </summary>
public class ForumModerator : BaseEntity
{
    public int ForumId { get; set; }
    public virtual Forum Forum { get; set; } = null!;

    public string UserId { get; set; } = string.Empty;
    public virtual ApplicationUser User { get; set; } = null!;

    public ForumModeratorRole Role { get; set; } = ForumModeratorRole.Moderator;
}

public enum ForumModeratorRole
{
    Moderator = 0,
    Admin = 1
}

/// <summary>
/// Forum topic tag
/// </summary>
public class ForumTopicTag
{
    public int TopicId { get; set; }
    public virtual ForumTopic Topic { get; set; } = null!;

    public int TagId { get; set; }
    public virtual Tag Tag { get; set; } = null!;
}

/// <summary>
/// Topic subscriber for notifications
/// </summary>
public class ForumTopicSubscriber : BaseEntity
{
    public int TopicId { get; set; }
    public virtual ForumTopic Topic { get; set; } = null!;

    public string UserId { get; set; } = string.Empty;
    public virtual ApplicationUser User { get; set; } = null!;

    public bool EmailNotification { get; set; } = true;
}

/// <summary>
/// Forum poll
/// </summary>
public class ForumPoll : BaseEntity
{
    public int TopicId { get; set; }
    public virtual ForumTopic Topic { get; set; } = null!;

    [Required]
    [MaxLength(300)]
    public string Question { get; set; } = string.Empty;

    public bool AllowMultiple { get; set; } = false;
    public bool ShowResults { get; set; } = true;

    public DateTime? ExpiresAt { get; set; }

    public int TotalVotes { get; set; } = 0;

    public virtual ICollection<ForumPollOption> Options { get; set; } = new List<ForumPollOption>();
}

/// <summary>
/// Forum poll option
/// </summary>
public class ForumPollOption : BaseEntity
{
    public int PollId { get; set; }
    public virtual ForumPoll Poll { get; set; } = null!;

    [Required]
    [MaxLength(200)]
    public string Text { get; set; } = string.Empty;

    public int VoteCount { get; set; } = 0;
    public int DisplayOrder { get; set; } = 0;

    public virtual ICollection<ForumPollVote> Votes { get; set; } = new List<ForumPollVote>();
}

/// <summary>
/// Forum poll vote
/// </summary>
public class ForumPollVote : BaseEntity
{
    public int OptionId { get; set; }
    public virtual ForumPollOption Option { get; set; } = null!;

    public string UserId { get; set; } = string.Empty;
    public virtual ApplicationUser User { get; set; } = null!;
}
