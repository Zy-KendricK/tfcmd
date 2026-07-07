using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class PagesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult ContactUs()
        {
            return View();
        }

        public IActionResult Gallery()
        {
            return View();
        }

        public IActionResult Groups()
        {
            return View();
        }
        // Add more actions for other pages as needed
    }
}
