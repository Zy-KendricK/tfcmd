using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppCore.Entities;

/// <summary>
/// Job listing
/// </summary>
public class Job : PublishableEntity
{
    [Required]
    [MaxLength(300)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Slug { get; set; }

    [Required]
    public string Description { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? CompanyName { get; set; }

    [MaxLength(500)]
    public string? CompanyLogoUrl { get; set; }

    [MaxLength(200)]
    public string? Location { get; set; }

    /// <summary>
    /// Job type: Full-time, Part-time, Contract, Freelance, Internship
    /// </summary>
    public JobType Type { get; set; } = JobType.FullTime;

    /// <summary>
    /// Remote work option
    /// </summary>
    public RemoteOption RemoteOption { get; set; } = RemoteOption.NoRemote;

    public int? CategoryId { get; set; }
    public virtual JobCategory? Category { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? SalaryMin { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? SalaryMax { get; set; }

    [MaxLength(10)]
    public string? SalaryCurrency { get; set; } = "USD";

    /// <summary>
    /// Salary period: Hourly, Daily, Weekly, Monthly, Yearly
    /// </summary>
    public SalaryPeriod? SalaryPeriod { get; set; }

    public bool ShowSalary { get; set; } = true;

    /// <summary>
    /// Application deadline
    /// </summary>
    public DateTime? ApplicationDeadline { get; set; }

    /// <summary>
    /// How to apply (URL or email)
    /// </summary>
    [MaxLength(500)]
    public string? ApplicationUrl { get; set; }

    [MaxLength(255)]
    public string? ApplicationEmail { get; set; }

    /// <summary>
    /// Required experience level
    /// </summary>
    public ExperienceLevel? ExperienceLevel { get; set; }

    /// <summary>
    /// Required skills (comma-separated or JSON)
    /// </summary>
    [MaxLength(1000)]
    public string? RequiredSkills { get; set; }

    /// <summary>
    /// Benefits offered
    /// </summary>
    [MaxLength(2000)]
    public string? Benefits { get; set; }

    public string PostedById { get; set; } = string.Empty;
    public virtual ApplicationUser PostedBy { get; set; } = null!;

    public bool IsFeatured { get; set; } = false;
    public bool IsUrgent { get; set; } = false;

    public int ViewCount { get; set; } = 0;
    public int ApplicationCount { get; set; } = 0;

    // Navigation properties
    public virtual ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
}

public enum JobType
{
    FullTime = 0,
    PartTime = 1,
    Contract = 2,
    Freelance = 3,
    Internship = 4,
    Temporary = 5
}

public enum RemoteOption
{
    NoRemote = 0,
    RemoteFriendly = 1,
    FullyRemote = 2,
    Hybrid = 3
}

public enum SalaryPeriod
{
    Hourly = 0,
    Daily = 1,
    Weekly = 2,
    Monthly = 3,
    Yearly = 4
}

public enum ExperienceLevel
{
    EntryLevel = 0,
    Junior = 1,
    MidLevel = 2,
    Senior = 3,
    Lead = 4,
    Executive = 5
}

/// <summary>
/// Job category
/// </summary>
public class JobCategory : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Slug { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(100)]
    public string? Icon { get; set; }

    public int? ParentCategoryId { get; set; }
    public virtual JobCategory? ParentCategory { get; set; }

    public int DisplayOrder { get; set; } = 0;

    public virtual ICollection<JobCategory> ChildCategories { get; set; } = new List<JobCategory>();
    public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();
}

/// <summary>
/// Job application
/// </summary>
public class JobApplication : BaseEntity
{
    public int JobId { get; set; }
    public virtual Job Job { get; set; } = null!;

    public string ApplicantId { get; set; } = string.Empty;
    public virtual ApplicationUser Applicant { get; set; } = null!;

    [MaxLength(500)]
    public string? CoverLetter { get; set; }

    [MaxLength(500)]
    public string? ResumeUrl { get; set; }

    public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;

    public string? Notes { get; set; }
}

public enum ApplicationStatus
{
    Pending = 0,
    Reviewed = 1,
    Shortlisted = 2,
    Interviewed = 3,
    Offered = 4,
    Hired = 5,
    Rejected = 6,
    Withdrawn = 7
}
