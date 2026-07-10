using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class PagesController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult ContactUs()
        {
            return RedirectToAction("Index", "Contact");
        }

        public IActionResult Gallery()
        {
            return View();
        }

        public IActionResult Groups()
        {
            return RedirectToActionPermanent("Index", "Groups");
        }
        // Add more actions for other pages as needed
    }
}
