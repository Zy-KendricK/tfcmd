using Microsoft.AspNetCore.Mvc;

namespace Web.ViewComponents
{
    public class LatestPostsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            // TODO: Replace with actual data from your database
            var posts = new[]
            {
                new { Title = "Community Update", Date = "July 30, 2025" },
                new { Title = "Recent Activities", Date = "July 28, 2025" }
            };

            return View(posts);
        }
    }
}
