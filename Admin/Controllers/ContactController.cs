using Microsoft.AspNetCore.Mvc;

namespace Admin.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            // List all contact messages
            return View();
        }

        public IActionResult Details(int id)
        {
            // Show contact message details
            return View();
        }

        public IActionResult Delete(int id)
        {
            // Delete contact message
            // TODO: Implement delete logic
            return RedirectToAction("Index");
        }
    }
}
