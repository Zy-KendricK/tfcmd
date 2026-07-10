using System.ComponentModel.DataAnnotations;

namespace AppCore.Entities;

/// <summary>
/// User group (role-based permission group)
/// Groups define what permissions users have in the system
/// </summary>
public class UserGroup : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(100)]
    public string? Icon { get; set; }

    [MaxLength(50)]
    public string? Color { get; set; }

    /// <summary>
    /// Display order for sorting
    /// </summary>
    public int DisplayOrder { get; set; } = 0;

    /// <summary>
    /// Whether this is a system group that cannot be deleted
    /// </summary>
    public bool IsSystemGroup { get; set; } = false;

    // Navigation properties
    public virtual ICollection<UserGroupMembership> Members { get; set; } = new List<UserGroupMembership>();
    public virtual ICollection<GroupPermission> Permissions { get; set; } = new List<GroupPermission>();
}

/// <summary>
/// Permission definition
/// </summary>
public class Permission : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Category for grouping permissions (e.g., "Users", "Posts", "Jobs")
    /// </summary>
    [MaxLength(50)]
    public string? Category { get; set; }

    /// <summary>
    /// Module this permission belongs to
    /// </summary>
    [MaxLength(50)]
    public string? Module { get; set; }

    public int DisplayOrder { get; set; } = 0;

    // Navigation properties
    public virtual ICollection<GroupPermission> GroupPermissions { get; set; } = new List<GroupPermission>();
}

/// <summary>
/// Many-to-many relationship between UserGroup and Permission
/// </summary>
public class GroupPermission
{
    public int Id { get; set; }

    public int UserGroupId { get; set; }
    public virtual UserGroup UserGroup { get; set; } = null!;

    public int PermissionId { get; set; }
    public virtual Permission Permission { get; set; } = null!;

    /// <summary>
    /// Whether this permission is granted (true) or denied (false)
    /// </summary>
    public bool IsGranted { get; set; } = true;

    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public string? AssignedById { get; set; }
}

/// <summary>
/// User membership in a group
/// </summary>
public class UserGroupMembership
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;
    public virtual ApplicationUser User { get; set; } = null!;

    public int UserGroupId { get; set; }
    public virtual UserGroup UserGroup { get; set; } = null!;

    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public string? AddedById { get; set; }

    /// <summary>
    /// Whether this is the user's primary group
    /// </summary>
    public bool IsPrimaryGroup { get; set; } = false;
}
