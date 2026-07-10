using AppCore;
using AppCore.Entities;
using AppCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Admin.Controllers;

[Authorize]
[RequirePermission("posts.view")]
public class CharityController : BaseAdminController
{
    private readonly ApplicationDbContext _context;

    public CharityController(
        ApplicationDbContext context,
        IPermissionService permissionService) : base(permissionService)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _context.CharityPageItems
            .Where(i => !i.IsDeleted)
            .OrderBy(i => i.DisplayOrder)
            .ThenBy(i => i.Id)
            .ToListAsync();

        var causes = await _context.CharityProjects
            .Where(p => !p.IsDeleted)
            .OrderBy(p => p.Id)
            .ToListAsync();

        var viewModel = new CharityPageManagerViewModel
        {
            SectionItems = items.GroupBy(i => i.Section)
                .ToDictionary(g => g.Key, g => g.ToList()),
            Causes = causes
        };

        return View(viewModel);
    }

    // ----- Charity page section items (banner, features, history, mission, counters, events, partners) -----

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("posts.create")]
    public async Task<IActionResult> SaveItem(CharityPageItemForm model)
    {
        if (string.IsNullOrWhiteSpace(model.Title) && string.IsNullOrWhiteSpace(model.Text))
        {
            TempData["Error"] = "Please provide at least a title or text for the item.";
            return RedirectToAction(nameof(Index));
        }

        CharityPageItem item;
        if (model.Id > 0)
        {
            var existing = await _context.CharityPageItems.FirstOrDefaultAsync(i => i.Id == model.Id && !i.IsDeleted);
            if (existing == null)
                return NotFound();

            item = existing;
            item.UpdatedById = CurrentUserId;
            item.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            item = new CharityPageItem
            {
                Section = model.Section,
                CreatedById = CurrentUserId,
                CreatedAt = DateTime.UtcNow,
                Status = ContentStatus.Published,
                IsPublishedToWeb = true,
                PublishedToWebAt = DateTime.UtcNow,
                PublishedById = CurrentUserId
            };
            _context.CharityPageItems.Add(item);
        }

        item.Title = model.Title?.Trim();
        item.Highlight = model.Highlight?.Trim();
        item.Text = model.Text?.Trim();
        item.ImageUrl = model.ImageUrl?.Trim();
        item.LinkUrl = model.LinkUrl?.Trim();
        item.Meta = model.Meta?.Trim();
        item.Number = model.Number;
        item.Date = model.Date;
        item.DisplayOrder = model.DisplayOrder;

        await _context.SaveChangesAsync();

        TempData["Success"] = model.Id > 0 ? "Section item updated." : "Section item added.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("posts.delete")]
    public async Task<IActionResult> DeleteItem(int id)
    {
        var item = await _context.CharityPageItems.FindAsync(id);
        if (item == null)
            return NotFound();

        item.IsDeleted = true;
        item.DeletedAt = DateTime.UtcNow;
        item.UpdatedById = CurrentUserId;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Section item deleted.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("posts.publish")]
    public async Task<IActionResult> ToggleItem(int id)
    {
        var item = await _context.CharityPageItems.FindAsync(id);
        if (item == null)
            return NotFound();

        if (item.IsPublishedToWeb)
        {
            item.Status = ContentStatus.Draft;
            item.IsPublishedToWeb = false;
        }
        else
        {
            item.Status = ContentStatus.Published;
            item.IsPublishedToWeb = true;
            item.PublishedToWebAt = DateTime.UtcNow;
            item.PublishedById = CurrentUserId;
        }

        item.UpdatedById = CurrentUserId;
        item.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        TempData["Success"] = item.IsPublishedToWeb ? "Item is now visible on the website." : "Item hidden from the website.";
        return RedirectToAction(nameof(Index));
    }

    // ----- Causes managed from the page manager -----

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("posts.create")]
    public async Task<IActionResult> SaveCause(CharityCauseForm model)
    {
        if (string.IsNullOrWhiteSpace(model.Title))
        {
            TempData["Error"] = "Please provide a title for the cause.";
            return RedirectToAction(nameof(Index));
        }

        CharityProject project;
        if (model.Id > 0)
        {
            var existing = await _context.CharityProjects.FirstOrDefaultAsync(p => p.Id == model.Id && !p.IsDeleted);
            if (existing == null)
                return NotFound();

            project = existing;
            project.UpdatedById = CurrentUserId;
            project.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            var charity = await GetOrCreateDefaultCharityAsync();
            project = new CharityProject
            {
                CharityId = charity.Id,
                CreatedById = CurrentUserId,
                CreatedAt = DateTime.UtcNow,
                Status = ContentStatus.Published,
                IsPublishedToWeb = true,
                PublishedToWebAt = DateTime.UtcNow,
                PublishedById = CurrentUserId,
                IsFeatured = true
            };
            _context.CharityProjects.Add(project);
        }

        project.Title = model.Title.Trim();
        project.Slug = GenerateSlug(model.Title);
        project.Description = model.Description?.Trim();
        project.ImageUrl = model.ImageUrl?.Trim();
        project.GoalAmount = model.GoalAmount;
        project.CurrentAmount = model.CurrentAmount;
        project.DonorCount = model.DonorCount ?? project.DonorCount;

        await _context.SaveChangesAsync();

        TempData["Success"] = model.Id > 0 ? "Cause updated." : "Cause added.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("posts.delete")]
    public async Task<IActionResult> DeleteCause(int id)
    {
        var project = await _context.CharityProjects.FindAsync(id);
        if (project == null)
            return NotFound();

        project.IsDeleted = true;
        project.DeletedAt = DateTime.UtcNow;
        project.UpdatedById = CurrentUserId;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Cause deleted.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("posts.publish")]
    public async Task<IActionResult> ToggleCause(int id)
    {
        var project = await _context.CharityProjects.FindAsync(id);
        if (project == null)
            return NotFound();

        if (project.IsPublishedToWeb)
        {
            project.Status = ContentStatus.Draft;
            project.IsPublishedToWeb = false;
        }
        else
        {
            project.Status = ContentStatus.Published;
            project.IsPublishedToWeb = true;
            project.PublishedToWebAt = DateTime.UtcNow;
            project.PublishedById = CurrentUserId;
        }

        project.UpdatedById = CurrentUserId;
        project.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        TempData["Success"] = project.IsPublishedToWeb ? "Cause is now visible on the website." : "Cause hidden from the website.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<Charity> GetOrCreateDefaultCharityAsync()
    {
        var charity = await _context.Charities
            .Where(c => !c.IsDeleted)
            .OrderBy(c => c.Id)
            .FirstOrDefaultAsync();

        if (charity != null)
            return charity;

        charity = new Charity
        {
            Name = "TFC Charity",
            Slug = "tfc-charity",
            Status = ContentStatus.Published,
            IsPublishedToWeb = true,
            PublishedToWebAt = DateTime.UtcNow,
            PublishedById = CurrentUserId,
            CreatedById = CurrentUserId,
            CreatedAt = DateTime.UtcNow
        };
        _context.Charities.Add(charity);
        await _context.SaveChangesAsync();
        return charity;
    }

    public async Task<IActionResult> Charities(string? search, int page = 1, int pageSize = 20)
    {
        var query = _context.Charities
            .Where(c => !c.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            search = search.ToLower();
            query = query.Where(c => c.Name.ToLower().Contains(search));
        }

        var totalCount = await query.CountAsync();
        var charities = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Sidebar stats
        ViewBag.TotalCharities = await _context.Charities.CountAsync(c => !c.IsDeleted);
        ViewBag.FeaturedCharities = await _context.Charities.CountAsync(c => !c.IsDeleted && c.IsFeatured);
        ViewBag.TotalProjects = await _context.CharityProjects.CountAsync(p => !p.IsDeleted);
        ViewBag.ActiveProjects = await _context.CharityProjects.CountAsync(p =>
            !p.IsDeleted && (p.EndDate == null || p.EndDate >= DateTime.UtcNow));

        var viewModel = new CharityListViewModel
        {
            Charities = charities,
            Search = search,
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        var charity = await _context.Charities
            .Include(c => c.Projects.Where(p => !p.IsDeleted))
            .FirstOrDefaultAsync(c => c.Id == id);

        if (charity == null)
            return NotFound();

        return View(charity);
    }

    [RequirePermission("posts.create")]
    public IActionResult Create()
    {
        return View(new CharityViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("posts.create")]
    public async Task<IActionResult> Create(CharityViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var charity = new Charity
        {
            Name = model.Name,
            Slug = GenerateSlug(model.Name),
            Tagline = model.Tagline,
            Description = model.Description,
            Website = model.Website,
            Email = model.Email,
            Phone = model.Phone,
            Address = model.Address,
            IsFeatured = model.IsFeatured,
            CreatedById = CurrentUserId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Charities.Add(charity);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Charity created successfully.";
        return RedirectToAction(nameof(Charities));
    }

    [RequirePermission("posts.edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var charity = await _context.Charities.FindAsync(id);
        if (charity == null)
            return NotFound();

        var model = new CharityViewModel
        {
            Id = charity.Id,
            Name = charity.Name,
            Tagline = charity.Tagline,
            Description = charity.Description,
            Website = charity.Website,
            Email = charity.Email,
            Phone = charity.Phone,
            Address = charity.Address,
            IsFeatured = charity.IsFeatured
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("posts.edit")]
    public async Task<IActionResult> Edit(CharityViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var charity = await _context.Charities.FindAsync(model.Id);
        if (charity == null)
            return NotFound();

        charity.Name = model.Name;
        charity.Tagline = model.Tagline;
        charity.Description = model.Description;
        charity.Website = model.Website;
        charity.Email = model.Email;
        charity.Phone = model.Phone;
        charity.Address = model.Address;
        charity.IsFeatured = model.IsFeatured;
        charity.UpdatedById = CurrentUserId;
        charity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        TempData["Success"] = "Charity updated successfully.";
        return RedirectToAction(nameof(Charities));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("posts.delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var charity = await _context.Charities.FindAsync(id);
        if (charity == null)
            return NotFound();

        charity.IsDeleted = true;
        charity.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Charity deleted.";
        return RedirectToAction(nameof(Charities));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("posts.publish")]
    public async Task<IActionResult> Publish(int id)
    {
        var charity = await _context.Charities.FindAsync(id);
        if (charity == null)
            return NotFound();

        charity.Status = ContentStatus.Published;
        charity.IsPublishedToWeb = true;
        charity.PublishedToWebAt = DateTime.UtcNow;
        charity.PublishedById = CurrentUserId;
        charity.UpdatedById = CurrentUserId;
        charity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Charity published to the website.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("posts.publish")]
    public async Task<IActionResult> Unpublish(int id)
    {
        var charity = await _context.Charities.FindAsync(id);
        if (charity == null)
            return NotFound();

        charity.Status = ContentStatus.Draft;
        charity.IsPublishedToWeb = false;
        charity.UpdatedById = CurrentUserId;
        charity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Charity unpublished from the website.";
        return RedirectToAction(nameof(Details), new { id });
    }

    // ----- Charity projects (causes shown on the website charity page) -----

    [RequirePermission("posts.create")]
    public async Task<IActionResult> CreateProject(int charityId)
    {
        var charity = await _context.Charities.FirstOrDefaultAsync(c => c.Id == charityId && !c.IsDeleted);
        if (charity == null)
            return NotFound();

        ViewBag.CharityName = charity.Name;
        return View(new CharityProjectViewModel { CharityId = charityId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("posts.create")]
    public async Task<IActionResult> CreateProject(CharityProjectViewModel model)
    {
        var charity = await _context.Charities.FirstOrDefaultAsync(c => c.Id == model.CharityId && !c.IsDeleted);
        if (charity == null)
            return NotFound();

        if (!ModelState.IsValid)
        {
            ViewBag.CharityName = charity.Name;
            return View(model);
        }

        var project = new CharityProject
        {
            CharityId = model.CharityId,
            Title = model.Title,
            Slug = GenerateSlug(model.Title),
            Description = model.Description,
            ImageUrl = model.ImageUrl,
            GoalAmount = model.GoalAmount,
            CurrentAmount = model.CurrentAmount,
            Currency = model.Currency,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            DonorCount = model.DonorCount,
            IsFeatured = model.IsFeatured,
            CreatedById = CurrentUserId,
            CreatedAt = DateTime.UtcNow
        };

        if (model.PublishToWeb)
        {
            project.Status = ContentStatus.Published;
            project.IsPublishedToWeb = true;
            project.PublishedToWebAt = DateTime.UtcNow;
            project.PublishedById = CurrentUserId;
        }

        _context.CharityProjects.Add(project);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Cause created successfully.";
        return RedirectToAction(nameof(Details), new { id = model.CharityId });
    }

    [RequirePermission("posts.edit")]
    public async Task<IActionResult> EditProject(int id)
    {
        var project = await _context.CharityProjects
            .Include(p => p.Charity)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        if (project == null)
            return NotFound();

        ViewBag.CharityName = project.Charity.Name;
        var model = new CharityProjectViewModel
        {
            Id = project.Id,
            CharityId = project.CharityId,
            Title = project.Title,
            Description = project.Description,
            ImageUrl = project.ImageUrl,
            GoalAmount = project.GoalAmount,
            CurrentAmount = project.CurrentAmount,
            Currency = project.Currency,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            DonorCount = project.DonorCount,
            IsFeatured = project.IsFeatured,
            PublishToWeb = project.IsPublishedToWeb
        };

        return View("CreateProject", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("posts.edit")]
    public async Task<IActionResult> EditProject(CharityProjectViewModel model)
    {
        var project = await _context.CharityProjects
            .Include(p => p.Charity)
            .FirstOrDefaultAsync(p => p.Id == model.Id && !p.IsDeleted);
        if (project == null)
            return NotFound();

        if (!ModelState.IsValid)
        {
            ViewBag.CharityName = project.Charity.Name;
            return View("CreateProject", model);
        }

        project.Title = model.Title;
        project.Slug = GenerateSlug(model.Title);
        project.Description = model.Description;
        project.ImageUrl = model.ImageUrl;
        project.GoalAmount = model.GoalAmount;
        project.CurrentAmount = model.CurrentAmount;
        project.Currency = model.Currency;
        project.StartDate = model.StartDate;
        project.EndDate = model.EndDate;
        project.DonorCount = model.DonorCount;
        project.IsFeatured = model.IsFeatured;
        project.UpdatedById = CurrentUserId;
        project.UpdatedAt = DateTime.UtcNow;

        if (model.PublishToWeb && !project.IsPublishedToWeb)
        {
            project.Status = ContentStatus.Published;
            project.IsPublishedToWeb = true;
            project.PublishedToWebAt = DateTime.UtcNow;
            project.PublishedById = CurrentUserId;
        }
        else if (!model.PublishToWeb && project.IsPublishedToWeb)
        {
            project.Status = ContentStatus.Draft;
            project.IsPublishedToWeb = false;
        }

        await _context.SaveChangesAsync();

        TempData["Success"] = "Cause updated successfully.";
        return RedirectToAction(nameof(Details), new { id = project.CharityId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("posts.delete")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var project = await _context.CharityProjects.FindAsync(id);
        if (project == null)
            return NotFound();

        project.IsDeleted = true;
        project.DeletedAt = DateTime.UtcNow;
        project.UpdatedById = CurrentUserId;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Cause deleted.";
        return RedirectToAction(nameof(Details), new { id = project.CharityId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("posts.publish")]
    public async Task<IActionResult> PublishProject(int id)
    {
        var project = await _context.CharityProjects.FindAsync(id);
        if (project == null)
            return NotFound();

        project.Status = ContentStatus.Published;
        project.IsPublishedToWeb = true;
        project.PublishedToWebAt = DateTime.UtcNow;
        project.PublishedById = CurrentUserId;
        project.UpdatedById = CurrentUserId;
        project.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Cause published to the website charity page.";
        return RedirectToAction(nameof(Details), new { id = project.CharityId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("posts.publish")]
    public async Task<IActionResult> UnpublishProject(int id)
    {
        var project = await _context.CharityProjects.FindAsync(id);
        if (project == null)
            return NotFound();

        project.Status = ContentStatus.Draft;
        project.IsPublishedToWeb = false;
        project.UpdatedById = CurrentUserId;
        project.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Cause removed from the website charity page.";
        return RedirectToAction(nameof(Details), new { id = project.CharityId });
    }

    private static string GenerateSlug(string title)
    {
        return title.ToLower()
            .Replace(" ", "-")
            .Replace("'", "")
            .Replace("\"", "");
    }
}

public class CharityListViewModel
{
    public IList<Charity> Charities { get; set; } = new List<Charity>();
    public string? Search { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}

public class CharityPageManagerViewModel
{
    public Dictionary<CharityPageSection, List<CharityPageItem>> SectionItems { get; set; } = new();
    public IList<CharityProject> Causes { get; set; } = new List<CharityProject>();

    public List<CharityPageItem> ItemsFor(CharityPageSection section) =>
        SectionItems.TryGetValue(section, out var items) ? items : new List<CharityPageItem>();
}

public class CharityPageItemForm
{
    public int Id { get; set; }
    public CharityPageSection Section { get; set; }
    public string? Title { get; set; }
    public string? Highlight { get; set; }
    public string? Text { get; set; }
    public string? ImageUrl { get; set; }
    public string? LinkUrl { get; set; }
    public string? Meta { get; set; }
    public int? Number { get; set; }
    public DateTime? Date { get; set; }
    public int DisplayOrder { get; set; }
}

public class CharityCauseForm
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? GoalAmount { get; set; }
    public decimal? CurrentAmount { get; set; }
    public int? DonorCount { get; set; }
}

public class CharityViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Tagline { get; set; }
    public string? Description { get; set; }
    public string? Website { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public bool IsFeatured { get; set; }
}

public class CharityProjectViewModel
{
    public int Id { get; set; }
    public int CharityId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? GoalAmount { get; set; }
    public decimal? CurrentAmount { get; set; }
    public string? Currency { get; set; } = "USD";
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int DonorCount { get; set; }
    public bool IsFeatured { get; set; }
    public bool PublishToWeb { get; set; }
}
