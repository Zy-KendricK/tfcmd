using Microsoft.AspNetCore.Mvc;

namespace Admin.Controllers
{
    public class CharityController : Controller
    {
        public IActionResult Index()
        {
            // List all charities
            return View();
        }

        // public IActionResult Create()
        // {
        //     // Show create charity form
        //     return View();
        // }

        [HttpPost]
        public IActionResult Create(/* CharityModel model */)
        {
            // Save new charity
            // TODO: Add model binding and validation
            return RedirectToAction("Index");
        }

        // public IActionResult Edit(int id)
        // {
        //     // Show edit charity form
        //     return View();
        // }

        [HttpPost]
        public IActionResult Edit(int id /*, CharityModel model */)
        {
            // Update charity
            // TODO: Add model binding and validation
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            // Delete charity
            // TODO: Implement delete logic
            return RedirectToAction("Index");
        }
    }
}
