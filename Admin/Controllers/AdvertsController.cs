using AppCore;
using AppCore.Entities;
using AppCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Admin.Controllers;

[Authorize]
[RequirePermission("adverts.view")]
public class AdvertsController : BaseAdminController
{
    private readonly ApplicationDbContext _context;
    private readonly IContentService<Advert> _advertService;

    public AdvertsController(
        ApplicationDbContext context,
        IPermissionService permissionService) : base(permissionService)
    {
        _context = context;
        _advertService = new ContentService<Advert>(context);
    }

    public async Task<IActionResult> Index(string? search, ContentStatus? status, int? categoryId, string? tab, string? sort, int page = 1, int pageSize = 20)
    {
        var query = _context.Adverts
            .Include(a => a.Category)
            .Include(a => a.PostedBy)
            .Include(a => a.Images)
            .Where(a => !a.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            search = search.ToLower();
            query = query.Where(a =>
                a.Title.ToLower().Contains(search) ||
                (a.Location != null && a.Location.ToLower().Contains(search)));
        }

        if (status.HasValue)
        {
            query = query.Where(a => a.Status == status.Value);
        }

        if (categoryId.HasValue)
        {
            query = query.Where(a => a.CategoryId == categoryId.Value);
        }

        query = tab switch
        {
            "pending" => query.Where(a => a.Status == ContentStatus.PendingReview),
            "live" => query.Where(a => a.IsPublishedToWeb),
            "featured" => query.Where(a => a.IsFeatured),
            "expired" => query.Where(a => a.ExpiresAt != null && a.ExpiresAt < DateTime.UtcNow),
            "internal" => query.Where(a => !a.IntendedForWeb),
            _ => query
        };

        query = sort switch
        {
            "price-asc" => query.OrderBy(a => a.Price ?? decimal.MaxValue),
            "price-desc" => query.OrderByDescending(a => a.Price ?? decimal.MinValue),
            "title" => query.OrderBy(a => a.Title),
            "oldest" => query.OrderBy(a => a.CreatedAt),
            _ => query.OrderByDescending(a => a.CreatedAt)
        };

        var totalCount = await query.CountAsync();
        var adverts = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.Categories = await GetCategoriesSelectListAsync();
        ViewBag.PendingReviewCount = await _context.Adverts.CountAsync(a => a.Status == ContentStatus.PendingReview && !a.IsDeleted);

        // Sidebar stats
        ViewBag.TotalAdverts = await _context.Adverts.CountAsync(a => !a.IsDeleted);
        ViewBag.PublishedAdverts = await _context.Adverts.CountAsync(a => !a.IsDeleted && a.IsPublishedToWeb);
        ViewBag.FeaturedAdverts = await _context.Adverts.CountAsync(a => !a.IsDeleted && a.IsFeatured);
        ViewBag.ExpiredAdverts = await _context.Adverts.CountAsync(a => !a.IsDeleted && a.ExpiresAt != null && a.ExpiresAt < DateTime.UtcNow);
        ViewBag.InternalAdverts = await _context.Adverts.CountAsync(a => !a.IsDeleted && !a.IntendedForWeb);
        ViewBag.CategoryCount = await _context.AdvertCategories.CountAsync(c => !c.IsDeleted);
        ViewBag.Tab = tab;
        ViewBag.Sort = sort;

        var viewModel = new AdvertListViewModel
        {
            Adverts = adverts,
            Search = search,
            Status = status,
            CategoryId = categoryId,
            Tab = tab,
            Sort = sort,
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        var advert = await _context.Adverts
            .Include(a => a.Category)
            .Include(a => a.PostedBy)
            .Include(a => a.Images)
            .Include(a => a.Inquiries)
                .ThenInclude(i => i.Inquirer)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (advert == null)
            return NotFound();

        return View(advert);
    }

    [RequirePermission("adverts.create")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await GetCategoriesSelectListAsync();
        return View(new AdvertViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("adverts.create")]
    public async Task<IActionResult> Create(AdvertViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await GetCategoriesSelectListAsync();
            return View(model);
        }

        var advert = new Advert
        {
            Title = model.Title,
            Slug = GenerateSlug(model.Title),
            Description = model.Description,
            CategoryId = model.CategoryId,
            Price = model.Price,
            Currency = model.Currency,
            PriceType = model.PriceType,
            IsNegotiable = model.IsNegotiable,
            Condition = model.Condition,
            Location = model.Location,
            PostedById = CurrentUserId!,
            ContactEmail = model.ContactEmail,
            ContactPhone = model.ContactPhone,
            ShowContactInfo = model.ShowContactInfo,
            IsFeatured = model.IsFeatured,
            IsUrgent = model.IsUrgent,
            ExpiresAt = model.ExpiresAt,
            IntendedForWeb = model.IntendedForWeb
        };

        await _advertService.CreateAsync(advert, CurrentUserId!);

        TempData["Success"] = "Advert created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [RequirePermission("adverts.edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var advert = await _context.Adverts.FindAsync(id);
        if (advert == null)
            return NotFound();

        var model = new AdvertViewModel
        {
            Id = advert.Id,
            Title = advert.Title,
            Description = advert.Description,
            CategoryId = advert.CategoryId,
            Price = advert.Price,
            Currency = advert.Currency,
            PriceType = advert.PriceType,
            IsNegotiable = advert.IsNegotiable,
            Condition = advert.Condition,
            Location = advert.Location,
            ContactEmail = advert.ContactEmail,
            ContactPhone = advert.ContactPhone,
            ShowContactInfo = advert.ShowContactInfo,
            IsFeatured = advert.IsFeatured,
            IsUrgent = advert.IsUrgent,
            ExpiresAt = advert.ExpiresAt,
            IntendedForWeb = advert.IntendedForWeb
        };

        ViewBag.Categories = await GetCategoriesSelectListAsync();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("adverts.edit")]
    public async Task<IActionResult> Edit(AdvertViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await GetCategoriesSelectListAsync();
            return View(model);
        }

        var advert = await _context.Adverts.FindAsync(model.Id);
        if (advert == null)
            return NotFound();

        advert.Title = model.Title;
        advert.Description = model.Description;
        advert.CategoryId = model.CategoryId;
        advert.Price = model.Price;
        advert.Currency = model.Currency;
        advert.PriceType = model.PriceType;
        advert.IsNegotiable = model.IsNegotiable;
        advert.Condition = model.Condition;
        advert.Location = model.Location;
        advert.ContactEmail = model.ContactEmail;
        advert.ContactPhone = model.ContactPhone;
        advert.ShowContactInfo = model.ShowContactInfo;
        advert.IsFeatured = model.IsFeatured;
        advert.IsUrgent = model.IsUrgent;
        advert.ExpiresAt = model.ExpiresAt;
        advert.IntendedForWeb = model.IntendedForWeb;
        if (!model.IntendedForWeb && advert.IsPublishedToWeb)
        {
            advert.IsPublishedToWeb = false;
            advert.Status = ContentStatus.Draft;
            TempData["Warning"] = "The advert was live on the website; withdrawing web intent has unpublished it.";
        }

        await _advertService.UpdateAsync(advert, CurrentUserId!);

        TempData["Success"] = "Advert updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("adverts.delete")]
    public async Task<IActionResult> Delete(int id)
    {
        await _advertService.DeleteAsync(id, CurrentUserId!);
        TempData["Success"] = "Advert deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("adverts.edit")]
    public async Task<IActionResult> RequestPublish(int id)
    {
        try
        {
            await _advertService.RequestPublishAsync(id, CurrentUserId!);
            TempData["Success"] = "Publish request submitted for review.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("adverts.publish")]
    public async Task<IActionResult> Approve(int id)
    {
        await _advertService.ApproveAsync(id, CurrentUserId!);
        TempData["Success"] = "Advert approved.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("adverts.publish")]
    public async Task<IActionResult> Reject(int id, string? notes)
    {
        await _advertService.RejectAsync(id, CurrentUserId!, notes ?? "Rejected from admin list");
        TempData["Success"] = "Advert rejected.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("adverts.publish")]
    public async Task<IActionResult> PublishToWeb(int id)
    {
        try
        {
            await _advertService.PublishToWebAsync(id, CurrentUserId!);
            TempData["Success"] = "Advert published to web.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("adverts.publish")]
    public async Task<IActionResult> Unpublish(int id)
    {
        await _advertService.UnpublishAsync(id, CurrentUserId!);
        TempData["Success"] = "Advert removed from the website.";
        return RedirectToAction(nameof(Index));
    }

    // Advert Categories
    public async Task<IActionResult> Categories()
    {
        var categories = await _context.AdvertCategories
            .Include(c => c.ParentCategory)
            .Where(c => !c.IsDeleted)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync();

        return View(categories);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("adverts.create")]
    public async Task<IActionResult> CreateCategory(string name, string? description, int? parentId)
    {
        var category = new AdvertCategory
        {
            Name = name,
            Slug = GenerateSlug(name),
            Description = description,
            ParentCategoryId = parentId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.AdvertCategories.Add(category);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Category created successfully.";
        return RedirectToAction(nameof(Categories));
    }

    private async Task<SelectList> GetCategoriesSelectListAsync()
    {
        var categories = await _context.AdvertCategories
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

public class AdvertListViewModel
{
    public IList<Advert> Adverts { get; set; } = new List<Advert>();
    public string? Search { get; set; }
    public ContentStatus? Status { get; set; }
    public int? CategoryId { get; set; }
    public string? Tab { get; set; }
    public string? Sort { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}

public class AdvertViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? CategoryId { get; set; }
    public decimal? Price { get; set; }
    public string? Currency { get; set; } = "USD";
    public PriceType PriceType { get; set; } = PriceType.Fixed;
    public bool IsNegotiable { get; set; }
    public AdvertCondition Condition { get; set; } = AdvertCondition.New;
    public string? Location { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public bool ShowContactInfo { get; set; } = true;
    public bool IsFeatured { get; set; }
    public bool IsUrgent { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IntendedForWeb { get; set; }
}
