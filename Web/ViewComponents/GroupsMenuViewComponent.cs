using AppCore;
using AppCore.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.ViewComponents
{
    /// <summary>
    /// Header sub-menu listing Charity plus all published groups.
    /// </summary>
    public class GroupsMenuViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public GroupsMenuViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var groups = await _context.SocialGroups
                .AsNoTracking()
                .Where(g => g.IsPublishedToWeb && g.IsActive && !g.IsDeleted
                            && (g.Slug == null || g.Slug != "charity")
                            && g.Name.ToLower() != "charity")
                .OrderBy(g => g.Name)
                .ToListAsync();

            return View(groups);
        }
    }
}
