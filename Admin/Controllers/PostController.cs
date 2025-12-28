using Microsoft.AspNetCore.Mvc;

namespace Admin.Controllers
{
    public class PostController : Controller
    {
        public IActionResult Index()
        {
            // List all posts
            return View();
        }

        // public IActionResult Create()
        // {
        //     // Show create post form
        //     return View();
        // }

        [HttpPost]
        public IActionResult Create(/* PostModel model */)
        {
            // Save new post
            // TODO: Add model binding and validation
            return RedirectToAction("Index");
        }

        // public IActionResult Edit(int id)
        // {
        //     // Show edit post form
        //     return View();
        // }

        [HttpPost]
        public IActionResult Edit(int id /*, PostModel model */)
        {
            // Update post
            // TODO: Add model binding and validation
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            // Delete post
            // TODO: Implement delete logic
            return RedirectToAction("Index");
        }
    }
}
