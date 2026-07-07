using System.ComponentModel.DataAnnotations;

namespace AppCore.Entities;

/// <summary>
/// Social group for members to join and interact
/// </summary>
public class SocialGroup : PublishableEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Slug { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }

    [MaxLength(500)]
    public string? AvatarUrl { get; set; }

    [MaxLength(500)]
    public string? CoverImageUrl { get; set; }

    /// <summary>
    /// Group type: Public, Private, Hidden
    /// </summary>
    public GroupType Type { get; set; } = GroupType.Public;

    /// <summary>
    /// Category of the group
    /// </summary>
    public int? CategoryId { get; set; }
    public virtual Category? Category { get; set; }

    /// <summary>
    /// Rules for the group
    /// </summary>
    [MaxLength(5000)]
    public string? Rules { get; set; }

    public int MemberCount { get; set; } = 0;

    // Navigation properties
    public virtual ICollection<SocialGroupMember> Members { get; set; } = new List<SocialGroupMember>();
    public virtual ICollection<SocialGroupPost> Posts { get; set; } = new List<SocialGroupPost>();
}

public enum GroupType
{
    Public = 0,
    Private = 1,
    Hidden = 2
}

/// <summary>
/// Member of a social group
/// </summary>
public class SocialGroupMember
{
    public int Id { get; set; }

    public int SocialGroupId { get; set; }
    public virtual SocialGroup SocialGroup { get; set; } = null!;

    public string UserId { get; set; } = string.Empty;
    public virtual ApplicationUser User { get; set; } = null!;

    /// <summary>
    /// Role in the group: Member, Moderator, Admin
    /// </summary>
    public GroupMemberRole Role { get; set; } = GroupMemberRole.Member;

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public string? InvitedById { get; set; }

    public bool IsApproved { get; set; } = true;
    public bool IsBanned { get; set; } = false;
    public DateTime? BannedAt { get; set; }
    public string? BanReason { get; set; }
}

public enum GroupMemberRole
{
    Member = 0,
    Moderator = 1,
    Admin = 2,
    Owner = 3
}

/// <summary>
/// Post within a social group
/// </summary>
public class SocialGroupPost : PublishableEntity
{
    public int SocialGroupId { get; set; }
    public virtual SocialGroup SocialGroup { get; set; } = null!;

    public string UserId { get; set; } = string.Empty;
    public virtual ApplicationUser User { get; set; } = null!;

    [Required]
    [MaxLength(5000)]
    public string Content { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? MediaUrl { get; set; }

    public bool IsPinned { get; set; } = false;

    public int LikeCount { get; set; } = 0;
    public int CommentCount { get; set; } = 0;
}
