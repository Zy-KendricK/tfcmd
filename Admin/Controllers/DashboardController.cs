using Microsoft.AspNetCore.Mvc;

namespace Admin.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
