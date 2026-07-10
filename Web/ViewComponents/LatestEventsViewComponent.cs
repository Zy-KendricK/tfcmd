using AppCore;
using AppCore.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.ViewComponents
{
	public class LatestEventsViewComponent : ViewComponent
	{
		private readonly ApplicationDbContext _context;

		public LatestEventsViewComponent(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			var events = await _context.Posts
				.AsNoTracking()
				.Where(p => p.IsPublishedToWeb && p.IsActive && !p.IsDeleted && p.HomeSection == HomeSection.Events)
				.OrderByDescending(p => p.PublishedToWebAt ?? p.CreatedAt)
				.Take(3)
				.ToListAsync();

			return View(events);
		}
	}
}
