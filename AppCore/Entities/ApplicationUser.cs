using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AppCore.Entities;

/// <summary>
/// Extended ApplicationUser with additional properties for the social platform
/// </summary>
public class ApplicationUser : IdentityUser
{
    [MaxLength(100)]
    public string? FirstName { get; set; }

    [MaxLength(100)]
    public string? LastName { get; set; }

    [MaxLength(255)]
    public string? DisplayName { get; set; }

    [MaxLength(500)]
    public string? Bio { get; set; }

    [MaxLength(500)]
    public string? AvatarUrl { get; set; }

    [MaxLength(500)]
    public string? CoverImageUrl { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [MaxLength(100)]
    public string? Location { get; set; }

    [MaxLength(255)]
    public string? Website { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Whether this user is the system super admin with absolute control
    /// </summary>
    public bool IsSuperAdmin { get; set; } = false;

    // Navigation properties
    public virtual ICollection<UserGroupMembership> GroupMemberships { get; set; } = new List<UserGroupMembership>();
    public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();
    public virtual ICollection<Advert> Adverts { get; set; } = new List<Advert>();
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    public virtual ICollection<Photo> Photos { get; set; } = new List<Photo>();
    public virtual ICollection<Video> Videos { get; set; } = new List<Video>();
    public virtual ICollection<ForumPost> ForumPosts { get; set; } = new List<ForumPost>();

    public string FullName => $"{FirstName} {LastName}".Trim();
}
