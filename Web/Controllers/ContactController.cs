using System.ComponentModel.DataAnnotations;
using AppCore;
using AppCore.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
	public class ContactController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly ILogger<ContactController> _logger;

		public ContactController(ApplicationDbContext context, ILogger<ContactController> logger)
		{
			_context = context;
			_logger = logger;
		}

		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Send(ContactFormModel form)
		{
			if (!ModelState.IsValid)
			{
				TempData["ContactError"] = "Please fill in all required fields correctly.";
				return RedirectToAction(nameof(Index));
			}

			try
			{
				var message = new ContactMessage
				{
					Name = form.Name.Trim(),
					Email = form.Email.Trim(),
					Phone = form.Phone?.Trim(),
					Message = form.Message.Trim(),
					IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
				};

				_context.ContactMessages.Add(message);
				await _context.SaveChangesAsync();

				TempData["ContactSuccess"] = "Thank you for reaching out! We will get back to you soon.";
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to save contact message from {Email}", form.Email);
				TempData["ContactError"] = "Something went wrong while sending your message. Please try again later.";
			}

			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Join(string? email)
		{
			if (string.IsNullOrWhiteSpace(email))
			{
				TempData["ContactError"] = "Please provide your email address to join.";
				return Redirect(Request.Headers.Referer.FirstOrDefault() ?? Url.Action(nameof(Index))!);
			}

			try
			{
				var message = new ContactMessage
				{
					Name = email.Trim(),
					Email = email.Trim(),
					Subject = "Membership Request",
					Message = "Request to join Tema Friends Club submitted from the website.",
					IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
				};

				_context.ContactMessages.Add(message);
				await _context.SaveChangesAsync();

				TempData["ContactSuccess"] = "Thank you! We'll contact you to continue the process.";
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to save join request from {Email}", email);
				TempData["ContactError"] = "Something went wrong while sending your request. Please try again later.";
			}

			return Redirect(Request.Headers.Referer.FirstOrDefault() ?? Url.Action(nameof(Index))!);
		}
	}

	public class ContactFormModel
	{
		[Required, StringLength(200)]
		public string Name { get; set; } = string.Empty;

		[Required, EmailAddress, StringLength(256)]
		public string Email { get; set; } = string.Empty;

		[StringLength(50)]
		public string? Phone { get; set; }

		[Required, StringLength(4000)]
		public string Message { get; set; } = string.Empty;
	}
}
