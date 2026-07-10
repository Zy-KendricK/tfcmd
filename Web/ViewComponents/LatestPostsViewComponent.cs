using AppCore;
using AppCore.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.ViewComponents
{
	public class LatestPostsViewComponent : ViewComponent
	{
		private readonly ApplicationDbContext _context;

		public LatestPostsViewComponent(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			var posts = await _context.Posts
				.AsNoTracking()
				.Where(p => p.IsPublishedToWeb && p.IsActive && !p.IsDeleted)
				.OrderByDescending(p => p.PublishedToWebAt ?? p.CreatedAt)
				.Take(3)
				.ToListAsync();

			return View(posts);
		}
	}
}
