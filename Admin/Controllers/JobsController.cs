using AppCore;
using AppCore.Entities;
using AppCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Admin.Controllers;

[Authorize]
[RequirePermission("jobs.view")]
public class JobsController : BaseAdminController
{
    private readonly ApplicationDbContext _context;
    private readonly IContentService<Job> _jobService;

    public JobsController(
        ApplicationDbContext context,
        IPermissionService permissionService) : base(permissionService)
    {
        _context = context;
        _jobService = new ContentService<Job>(context);
    }

    public async Task<IActionResult> Index(string? search, ContentStatus? status, int? categoryId, int page = 1, int pageSize = 20)
    {
        var query = _context.Jobs
            .Include(j => j.Category)
            .Include(j => j.PostedBy)
            .Where(j => !j.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            search = search.ToLower();
            query = query.Where(j =>
                j.Title.ToLower().Contains(search) ||
                (j.CompanyName != null && j.CompanyName.ToLower().Contains(search)) ||
                (j.Location != null && j.Location.ToLower().Contains(search)));
        }

        if (status.HasValue)
        {
            query = query.Where(j => j.Status == status.Value);
        }

        if (categoryId.HasValue)
        {
            query = query.Where(j => j.CategoryId == categoryId.Value);
        }

        var totalCount = await query.CountAsync();
        var jobs = await query
            .OrderByDescending(j => j.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.Categories = await GetCategoriesSelectListAsync();
        ViewBag.PendingReviewCount = await _context.Jobs.CountAsync(j => j.Status == ContentStatus.PendingReview && !j.IsDeleted);

        var viewModel = new JobListViewModel
        {
            Jobs = jobs,
            Search = search,
            Status = status,
            CategoryId = categoryId,
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };

        return View(viewModel);
    }

    public async Task<IActionResult> PendingReview(int page = 1, int pageSize = 20)
    {
        var jobs = await _context.Jobs
            .Include(j => j.Category)
            .Include(j => j.PostedBy)
            .Where(j => j.Status == ContentStatus.PendingReview && !j.IsDeleted)
            .OrderByDescending(j => j.PublishRequestedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var totalCount = await _context.Jobs.CountAsync(j => j.Status == ContentStatus.PendingReview && !j.IsDeleted);

        ViewBag.TotalCount = totalCount;
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return View(jobs);
    }

    public async Task<IActionResult> Details(int id)
    {
        var job = await _context.Jobs
            .Include(j => j.Category)
            .Include(j => j.PostedBy)
            .Include(j => j.Applications)
                .ThenInclude(a => a.Applicant)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (job == null)
            return NotFound();

        return View(job);
    }

    [RequirePermission("jobs.create")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await GetCategoriesSelectListAsync();
        return View(new JobViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("jobs.create")]
    public async Task<IActionResult> Create(JobViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await GetCategoriesSelectListAsync();
            return View(model);
        }

        var job = new Job
        {
            Title = model.Title,
            Slug = GenerateSlug(model.Title),
            Description = model.Description,
            CompanyName = model.CompanyName,
            Location = model.Location,
            Type = model.Type,
            RemoteOption = model.RemoteOption,
            CategoryId = model.CategoryId,
            SalaryMin = model.SalaryMin,
            SalaryMax = model.SalaryMax,
            SalaryCurrency = model.SalaryCurrency,
            SalaryPeriod = model.SalaryPeriod,
            ShowSalary = model.ShowSalary,
            ApplicationDeadline = model.ApplicationDeadline,
            ApplicationUrl = model.ApplicationUrl,
            ApplicationEmail = model.ApplicationEmail,
            ExperienceLevel = model.ExperienceLevel,
            RequiredSkills = model.RequiredSkills,
            Benefits = model.Benefits,
            PostedById = CurrentUserId!,
            IsFeatured = model.IsFeatured,
            IsUrgent = model.IsUrgent
        };

        await _jobService.CreateAsync(job, CurrentUserId!);

        TempData["Success"] = "Job created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [RequirePermission("jobs.edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var job = await _context.Jobs.FindAsync(id);
        if (job == null)
            return NotFound();

        var model = new JobViewModel
        {
            Id = job.Id,
            Title = job.Title,
            Description = job.Description,
            CompanyName = job.CompanyName,
            Location = job.Location,
            Type = job.Type,
            RemoteOption = job.RemoteOption,
            CategoryId = job.CategoryId,
            SalaryMin = job.SalaryMin,
            SalaryMax = job.SalaryMax,
            SalaryCurrency = job.SalaryCurrency,
            SalaryPeriod = job.SalaryPeriod,
            ShowSalary = job.ShowSalary,
            ApplicationDeadline = job.ApplicationDeadline,
            ApplicationUrl = job.ApplicationUrl,
            ApplicationEmail = job.ApplicationEmail,
            ExperienceLevel = job.ExperienceLevel,
            RequiredSkills = job.RequiredSkills,
            Benefits = job.Benefits,
            IsFeatured = job.IsFeatured,
            IsUrgent = job.IsUrgent
        };

        ViewBag.Categories = await GetCategoriesSelectListAsync();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("jobs.edit")]
    public async Task<IActionResult> Edit(JobViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await GetCategoriesSelectListAsync();
            return View(model);
        }

        var job = await _context.Jobs.FindAsync(model.Id);
        if (job == null)
            return NotFound();

        job.Title = model.Title;
        job.Description = model.Description;
        job.CompanyName = model.CompanyName;
        job.Location = model.Location;
        job.Type = model.Type;
        job.RemoteOption = model.RemoteOption;
        job.CategoryId = model.CategoryId;
        job.SalaryMin = model.SalaryMin;
        job.SalaryMax = model.SalaryMax;
        job.SalaryCurrency = model.SalaryCurrency;
        job.SalaryPeriod = model.SalaryPeriod;
        job.ShowSalary = model.ShowSalary;
        job.ApplicationDeadline = model.ApplicationDeadline;
        job.ApplicationUrl = model.ApplicationUrl;
        job.ApplicationEmail = model.ApplicationEmail;
        job.ExperienceLevel = model.ExperienceLevel;
        job.RequiredSkills = model.RequiredSkills;
        job.Benefits = model.Benefits;
        job.IsFeatured = model.IsFeatured;
        job.IsUrgent = model.IsUrgent;

        await _jobService.UpdateAsync(job, CurrentUserId!);

        TempData["Success"] = "Job updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("jobs.delete")]
    public async Task<IActionResult> Delete(int id)
    {
        await _jobService.DeleteAsync(id, CurrentUserId!);
        TempData["Success"] = "Job deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("jobs.edit")]
    public async Task<IActionResult> RequestPublish(int id)
    {
        await _jobService.RequestPublishAsync(id, CurrentUserId!);
        TempData["Success"] = "Publish request submitted.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("jobs.publish")]
    public async Task<IActionResult> Approve(int id, string? notes)
    {
        await _jobService.ApproveAsync(id, CurrentUserId!, notes);
        TempData["Success"] = "Job approved.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("jobs.publish")]
    public async Task<IActionResult> Reject(int id, string notes)
    {
        if (string.IsNullOrEmpty(notes))
        {
            TempData["Error"] = "Please provide a reason for rejection.";
            return RedirectToAction(nameof(Details), new { id });
        }

        await _jobService.RejectAsync(id, CurrentUserId!, notes);
        TempData["Success"] = "Job rejected.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("jobs.publish")]
    public async Task<IActionResult> PublishToWeb(int id)
    {
        await _jobService.PublishToWebAsync(id, CurrentUserId!);
        TempData["Success"] = "Job published to web.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("jobs.publish")]
    public async Task<IActionResult> Unpublish(int id)
    {
        await _jobService.UnpublishAsync(id, CurrentUserId!);
        TempData["Success"] = "Job unpublished.";
        return RedirectToAction(nameof(Details), new { id });
    }

    // Job Categories
    public async Task<IActionResult> Categories()
    {
        var categories = await _context.JobCategories
            .Include(c => c.ParentCategory)
            .Where(c => !c.IsDeleted)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync();

        return View(categories);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCategory(string name, string? description, int? parentId)
    {
        var category = new JobCategory
        {
            Name = name,
            Slug = GenerateSlug(name),
            Description = description,
            ParentCategoryId = parentId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.JobCategories.Add(category);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Category created successfully.";
        return RedirectToAction(nameof(Categories));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _context.JobCategories.FindAsync(id);
        if (category != null)
        {
            category.IsDeleted = true;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Category deleted.";
        }
        return RedirectToAction(nameof(Categories));
    }

    private async Task<SelectList> GetCategoriesSelectListAsync()
    {
        var categories = await _context.JobCategories
            .Where(c => c.IsActive && !c.IsDeleted)
            .OrderBy(c => c.Name)
            .ToListAsync();

        return new SelectList(categories, "Id", "Name");
    }

    private static string GenerateSlug(string title)
    {
        return title.ToLower()
            .Replace(" ", "-")
            .Replace("'", "")
            .Replace("\"", "");
    }
}

public class JobListViewModel
{
    public IList<Job> Jobs { get; set; } = new List<Job>();
    public string? Search { get; set; }
    public ContentStatus? Status { get; set; }
    public int? CategoryId { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}

public class JobViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string? Location { get; set; }
    public JobType Type { get; set; } = JobType.FullTime;
    public RemoteOption RemoteOption { get; set; } = RemoteOption.NoRemote;
    public int? CategoryId { get; set; }
    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public string? SalaryCurrency { get; set; } = "USD";
    public SalaryPeriod? SalaryPeriod { get; set; }
    public bool ShowSalary { get; set; } = true;
    public DateTime? ApplicationDeadline { get; set; }
    public string? ApplicationUrl { get; set; }
    public string? ApplicationEmail { get; set; }
    public ExperienceLevel? ExperienceLevel { get; set; }
    public string? RequiredSkills { get; set; }
    public string? Benefits { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsUrgent { get; set; }
}
