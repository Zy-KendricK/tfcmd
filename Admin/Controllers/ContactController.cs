using AppCore;
using AppCore.Entities;
using AppCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Admin.Controllers;

[Authorize]
[RequirePermission("posts.view")]
public class ContactController : BaseAdminController
{
	private readonly ApplicationDbContext _context;

	public ContactController(
		ApplicationDbContext context,
		IPermissionService permissionService) : base(permissionService)
	{
		_context = context;
	}

	public async Task<IActionResult> Index(string? search, ContactMessageStatus? status, int page = 1, int pageSize = 20)
	{
		var query = _context.ContactMessages
			.Include(m => m.Category)
			.Where(m => !m.IsDeleted)
			.AsQueryable();

		if (!string.IsNullOrEmpty(search))
		{
			search = search.ToLower();
			query = query.Where(m =>
				m.Name.ToLower().Contains(search) ||
				m.Email.ToLower().Contains(search) ||
				(m.Subject != null && m.Subject.ToLower().Contains(search)));
		}

		if (status.HasValue)
		{
			query = query.Where(m => m.Status == status.Value);
		}

		var totalCount = await query.CountAsync();
		var messages = await query
			.OrderByDescending(m => m.CreatedAt)
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync();

		// Sidebar stats
		ViewBag.TotalMessages = await _context.ContactMessages.CountAsync(m => !m.IsDeleted);
		ViewBag.UnreadMessages = await _context.ContactMessages.CountAsync(m => !m.IsDeleted && !m.IsRead);
		ViewBag.NewMessages = await _context.ContactMessages.CountAsync(m => !m.IsDeleted && m.Status == ContactMessageStatus.New);
		ViewBag.ResolvedMessages = await _context.ContactMessages.CountAsync(m => !m.IsDeleted && m.Status == ContactMessageStatus.Resolved);

		var viewModel = new ContactListViewModel
		{
			Messages = messages,
			Search = search,
			Status = status,
			CurrentPage = page,
			PageSize = pageSize,
			TotalCount = totalCount,
			TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
		};

		return View(viewModel);
	}

	public async Task<IActionResult> Details(int id)
	{
		var message = await _context.ContactMessages
			.Include(m => m.Category)
			.Include(m => m.User)
			.Include(m => m.AssignedTo)
			.Include(m => m.Replies)
			.FirstOrDefaultAsync(m => m.Id == id);

		if (message == null)
			return NotFound();

		if (!message.IsRead)
		{
			message.IsRead = true;
			message.ReadAt = DateTime.UtcNow;
			await _context.SaveChangesAsync();
		}

		return View(message);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> UpdateStatus(int id, ContactMessageStatus status)
	{
		var message = await _context.ContactMessages.FindAsync(id);
		if (message == null)
			return NotFound();

		message.Status = status;
		message.UpdatedAt = DateTime.UtcNow;
		await _context.SaveChangesAsync();

		TempData["Success"] = "Message status updated.";
		return RedirectToAction(nameof(Details), new { id });
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	[RequirePermission("posts.delete")]
	public async Task<IActionResult> Delete(int id)
	{
		var message = await _context.ContactMessages.FindAsync(id);
		if (message == null)
			return NotFound();

		message.IsDeleted = true;
		message.DeletedAt = DateTime.UtcNow;
		await _context.SaveChangesAsync();

		TempData["Success"] = "Message deleted.";
		return RedirectToAction(nameof(Index));
	}
}

public class ContactListViewModel
{
	public IList<ContactMessage> Messages { get; set; } = new List<ContactMessage>();
	public string? Search { get; set; }
	public ContactMessageStatus? Status { get; set; }
	public int CurrentPage { get; set; }
	public int PageSize { get; set; }
	public int TotalCount { get; set; }
	public int TotalPages { get; set; }
}
