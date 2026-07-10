using AppCore;
using AppCore.Entities;
using AppCore.Services;
using Admin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Admin.Controllers;

[Authorize]
public class SocialGroupsController : BaseAdminController
{
    private readonly ApplicationDbContext _context;
    private readonly IImageUploadService _imageUpload;

    public SocialGroupsController(
        ApplicationDbContext context,
        IPermissionService permissionService,
        IImageUploadService imageUpload) : base(permissionService)
    {
        _context = context;
        _imageUpload = imageUpload;
    }

    public async Task<IActionResult> Index(string? search, string? type, int page = 1, int pageSize = 20)
    {
        var query = _context.SocialGroups
            .Where(g => !g.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            search = search.ToLower();
            query = query.Where(g =>
                g.Name.ToLower().Contains(search) ||
                (g.Description != null && g.Description.ToLower().Contains(search)));
        }

        if (!string.IsNullOrEmpty(type) && Enum.TryParse<GroupType>(type, out var groupType))
        {
            query = query.Where(g => g.Type == groupType);
        }

        var totalCount = await query.CountAsync();
        var groups = await query
            .OrderByDescending(g => g.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Get member counts
        var groupIds = groups.Select(g => g.Id).ToList();
        var memberCounts = await _context.SocialGroupMembers
            .Where(m => groupIds.Contains(m.SocialGroupId) && !m.IsBanned)
            .GroupBy(m => m.SocialGroupId)
            .Select(g => new { GroupId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.GroupId, x => x.Count);

        // Sidebar stats
        ViewBag.TotalGroups = await _context.SocialGroups.CountAsync(g => !g.IsDeleted);
        ViewBag.PublicGroups = await _context.SocialGroups.CountAsync(g => !g.IsDeleted && g.Type == GroupType.Public);
        ViewBag.PrivateGroups = await _context.SocialGroups.CountAsync(g => !g.IsDeleted && g.Type == GroupType.Private);
        ViewBag.HiddenGroups = await _context.SocialGroups.CountAsync(g => !g.IsDeleted && g.Type == GroupType.Hidden);
        ViewBag.TotalMemberships = await _context.SocialGroupMembers.CountAsync(m => !m.IsBanned);

        var viewModel = new SocialGroupListViewModel
        {
            Groups = groups,
            MemberCounts = memberCounts,
            Search = search,
            Type = type,
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        var group = await _context.SocialGroups
            .Include(g => g.Members)
                .ThenInclude(m => m.User)
            .Include(g => g.Posts)
                .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (group == null)
            return NotFound();

        return View(group);
    }

    public IActionResult Create()
    {
        return View(new SocialGroupViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SocialGroupViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var avatarUrl = await _imageUpload.SaveAsync(model.AvatarFile, "groups");
        var coverUrl = await _imageUpload.SaveAsync(model.CoverFile, "groups");

        var group = new SocialGroup
        {
            Name = model.Name,
            Slug = GenerateSlug(model.Name),
            Description = model.Description,
            Type = model.Type,
            Rules = model.Rules,
            AvatarUrl = avatarUrl ?? model.AvatarUrl,
            CoverImageUrl = coverUrl ?? model.CoverImageUrl,
            CreatedById = CurrentUserId!,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.SocialGroups.Add(group);
        await _context.SaveChangesAsync();

        // Add creator as owner
        _context.SocialGroupMembers.Add(new SocialGroupMember
        {
            SocialGroupId = group.Id,
            UserId = CurrentUserId!,
            Role = GroupMemberRole.Owner,
            IsApproved = true,
            JoinedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        TempData["Success"] = "Group created successfully.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var group = await _context.SocialGroups.FindAsync(id);
        if (group == null)
            return NotFound();

        var model = new SocialGroupViewModel
        {
            Id = group.Id,
            Name = group.Name,
            Description = group.Description,
            Type = group.Type,
            Rules = group.Rules,
            AvatarUrl = group.AvatarUrl,
            CoverImageUrl = group.CoverImageUrl,
            IsActive = group.IsActive
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(SocialGroupViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var group = await _context.SocialGroups.FindAsync(model.Id);
        if (group == null)
            return NotFound();

        var avatarUrl = await _imageUpload.SaveAsync(model.AvatarFile, "groups");
        var coverUrl = await _imageUpload.SaveAsync(model.CoverFile, "groups");

        group.Name = model.Name;
        group.Description = model.Description;
        group.Type = model.Type;
        group.Rules = model.Rules;
        group.AvatarUrl = avatarUrl ?? model.AvatarUrl;
        group.CoverImageUrl = coverUrl ?? model.CoverImageUrl;
        group.IsActive = model.IsActive;
        group.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        TempData["Success"] = "Group updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var group = await _context.SocialGroups.FindAsync(id);
        if (group == null)
            return NotFound();

        group.IsDeleted = true;
        group.UpdatedAt = DateTime.UtcNow;
        await HomeContentService.TouchCacheVersionAsync(_context, CurrentUserId);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Group deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RequestPublish(int id)
    {
        var group = await _context.SocialGroups.FindAsync(id);
        if (group == null)
            return NotFound();

        group.PublishRequested = true;
        group.PublishRequestedAt = DateTime.UtcNow;
        group.Status = ContentStatus.PendingReview;
        group.UpdatedById = CurrentUserId;
        group.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Publish request submitted for review.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("posts.publish")]
    public async Task<IActionResult> Approve(int id)
    {
        var group = await _context.SocialGroups.FindAsync(id);
        if (group == null)
            return NotFound();

        group.Status = ContentStatus.Approved;
        group.ReviewedById = CurrentUserId;
        group.ReviewedAt = DateTime.UtcNow;
        group.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Group approved. It can now be published to the website.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("posts.publish")]
    public async Task<IActionResult> PublishToWeb(int id)
    {
        var group = await _context.SocialGroups.FindAsync(id);
        if (group == null)
            return NotFound();

        group.Status = ContentStatus.Published;
        group.IsPublishedToWeb = true;
        group.PublishedToWebAt = DateTime.UtcNow;
        group.PublishedById = CurrentUserId;
        group.UpdatedAt = DateTime.UtcNow;
        await HomeContentService.TouchCacheVersionAsync(_context, CurrentUserId);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Group published to the website.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("posts.publish")]
    public async Task<IActionResult> Unpublish(int id)
    {
        var group = await _context.SocialGroups.FindAsync(id);
        if (group == null)
            return NotFound();

        group.IsPublishedToWeb = false;
        group.Status = ContentStatus.Draft;
        group.UpdatedById = CurrentUserId;
        group.UpdatedAt = DateTime.UtcNow;
        await HomeContentService.TouchCacheVersionAsync(_context, CurrentUserId);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Group removed from the website.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Members(int id)
    {
        var group = await _context.SocialGroups
            .Include(g => g.Members)
                .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (group == null)
            return NotFound();

        return View(group);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateMemberRole(int groupId, string userId, GroupMemberRole role)
    {
        var member = await _context.SocialGroupMembers
            .FirstOrDefaultAsync(m => m.SocialGroupId == groupId && m.UserId == userId);

        if (member == null)
            return NotFound();

        member.Role = role;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Member role updated.";
        return RedirectToAction(nameof(Members), new { id = groupId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveMember(int groupId, string userId)
    {
        var member = await _context.SocialGroupMembers
            .FirstOrDefaultAsync(m => m.SocialGroupId == groupId && m.UserId == userId);

        if (member == null)
            return NotFound();

        _context.SocialGroupMembers.Remove(member);
        await _context.SaveChangesAsync();

        // Update member count
        var group = await _context.SocialGroups.FindAsync(groupId);
        if (group != null)
        {
            group.MemberCount = await _context.SocialGroupMembers
                .CountAsync(m => m.SocialGroupId == groupId && !m.IsBanned);
            await _context.SaveChangesAsync();
        }

        TempData["Success"] = "Member removed from group.";
        return RedirectToAction(nameof(Members), new { id = groupId });
    }

    private static string GenerateSlug(string name)
    {
        return name.ToLower()
            .Replace(" ", "-")
            .Replace("'", "")
            .Replace("\"", "");
    }
}

public class SocialGroupListViewModel
{
    public IList<SocialGroup> Groups { get; set; } = new List<SocialGroup>();
    public Dictionary<int, int> MemberCounts { get; set; } = new();
    public string? Search { get; set; }
    public string? Type { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}

public class SocialGroupViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public GroupType Type { get; set; } = GroupType.Public;
    public string? Rules { get; set; }
    public string? AvatarUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    public IFormFile? AvatarFile { get; set; }
    public IFormFile? CoverFile { get; set; }
    public bool IsActive { get; set; } = true;
}
