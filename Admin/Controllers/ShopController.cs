using AppCore;
using AppCore.Entities;
using AppCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Admin.Controllers;

[Authorize]
[RequirePermission("products.view")]
public class ShopController : BaseAdminController
{
    private readonly ApplicationDbContext _context;
    private readonly IContentService<Product> _productService;

    public ShopController(
        ApplicationDbContext context,
        IPermissionService permissionService) : base(permissionService)
    {
        _context = context;
        _productService = new ContentService<Product>(context);
    }

    public async Task<IActionResult> Index(string? search, ContentStatus? status, int? categoryId, int page = 1, int pageSize = 20)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Seller)
            .Where(p => !p.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            search = search.ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(search) ||
                (p.SKU != null && p.SKU.ToLower().Contains(search)));
        }

        if (status.HasValue)
        {
            query = query.Where(p => p.Status == status.Value);
        }

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        var totalCount = await query.CountAsync();
        var products = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.Categories = await GetCategoriesSelectListAsync();
        ViewBag.PendingReviewCount = await _context.Products.CountAsync(p => p.Status == ContentStatus.PendingReview && !p.IsDeleted);

        var viewModel = new ProductListViewModel
        {
            Products = products,
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

    public async Task<IActionResult> Details(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Seller)
            .Include(p => p.Images)
            .Include(p => p.Variants)
            .Include(p => p.Reviews)
                .ThenInclude(r => r.Reviewer)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return NotFound();

        return View(product);
    }

    [RequirePermission("products.create")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await GetCategoriesSelectListAsync();
        return View(new ProductViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("products.create")]
    public async Task<IActionResult> Create(ProductViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await GetCategoriesSelectListAsync();
            return View(model);
        }

        var product = new Product
        {
            Name = model.Name,
            Slug = GenerateSlug(model.Name),
            SKU = model.SKU,
            ShortDescription = model.ShortDescription,
            Description = model.Description,
            CategoryId = model.CategoryId,
            Price = model.Price,
            CompareAtPrice = model.CompareAtPrice,
            CostPrice = model.CostPrice,
            Currency = model.Currency,
            Type = model.Type,
            StockQuantity = model.StockQuantity,
            TrackInventory = model.TrackInventory,
            AllowBackorder = model.AllowBackorder,
            Weight = model.Weight,
            SellerId = CurrentUserId!,
            IsFeatured = model.IsFeatured,
            IsOnSale = model.IsOnSale
        };

        await _productService.CreateAsync(product, CurrentUserId!);

        TempData["Success"] = "Product created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [RequirePermission("products.edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return NotFound();

        var model = new ProductViewModel
        {
            Id = product.Id,
            Name = product.Name,
            SKU = product.SKU,
            ShortDescription = product.ShortDescription,
            Description = product.Description,
            CategoryId = product.CategoryId,
            Price = product.Price,
            CompareAtPrice = product.CompareAtPrice,
            CostPrice = product.CostPrice,
            Currency = product.Currency,
            Type = product.Type,
            StockQuantity = product.StockQuantity,
            TrackInventory = product.TrackInventory,
            AllowBackorder = product.AllowBackorder,
            Weight = product.Weight,
            IsFeatured = product.IsFeatured,
            IsOnSale = product.IsOnSale
        };

        ViewBag.Categories = await GetCategoriesSelectListAsync();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("products.edit")]
    public async Task<IActionResult> Edit(ProductViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await GetCategoriesSelectListAsync();
            return View(model);
        }

        var product = await _context.Products.FindAsync(model.Id);
        if (product == null)
            return NotFound();

        product.Name = model.Name;
        product.SKU = model.SKU;
        product.ShortDescription = model.ShortDescription;
        product.Description = model.Description;
        product.CategoryId = model.CategoryId;
        product.Price = model.Price;
        product.CompareAtPrice = model.CompareAtPrice;
        product.CostPrice = model.CostPrice;
        product.Currency = model.Currency;
        product.Type = model.Type;
        product.StockQuantity = model.StockQuantity;
        product.TrackInventory = model.TrackInventory;
        product.AllowBackorder = model.AllowBackorder;
        product.Weight = model.Weight;
        product.IsFeatured = model.IsFeatured;
        product.IsOnSale = model.IsOnSale;

        await _productService.UpdateAsync(product, CurrentUserId!);

        TempData["Success"] = "Product updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("products.delete")]
    public async Task<IActionResult> Delete(int id)
    {
        await _productService.DeleteAsync(id, CurrentUserId!);
        TempData["Success"] = "Product deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("products.publish")]
    public async Task<IActionResult> PublishToWeb(int id)
    {
        await _productService.PublishToWebAsync(id, CurrentUserId!);
        TempData["Success"] = "Product published to web.";
        return RedirectToAction(nameof(Details), new { id });
    }

    // Product Categories
    public async Task<IActionResult> Categories()
    {
        var categories = await _context.ProductCategories
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
        var category = new ProductCategory
        {
            Name = name,
            Slug = GenerateSlug(name),
            Description = description,
            ParentCategoryId = parentId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.ProductCategories.Add(category);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Category created successfully.";
        return RedirectToAction(nameof(Categories));
    }

    private async Task<SelectList> GetCategoriesSelectListAsync()
    {
        var categories = await _context.ProductCategories
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

public class ProductListViewModel
{
    public IList<Product> Products { get; set; } = new List<Product>();
    public string? Search { get; set; }
    public ContentStatus? Status { get; set; }
    public int? CategoryId { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}

public class ProductViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? SKU { get; set; }
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }
    public int? CategoryId { get; set; }
    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public decimal? CostPrice { get; set; }
    public string Currency { get; set; } = "USD";
    public ProductType Type { get; set; } = ProductType.Physical;
    public int? StockQuantity { get; set; }
    public bool TrackInventory { get; set; } = true;
    public bool AllowBackorder { get; set; }
    public double? Weight { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsOnSale { get; set; }
}
