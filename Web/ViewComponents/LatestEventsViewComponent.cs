using Microsoft.AspNetCore.Mvc;

namespace Web.ViewComponents
{
    public class LatestEventsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            // TODO: Replace with actual data from your database
            var events = new[]
            {
                new { Title = "Annual Charity Gala", Date = "August 15, 2025" },
                new { Title = "Community Outreach", Date = "September 1, 2025" }
            };

            return View(events);
        }
    }
}
