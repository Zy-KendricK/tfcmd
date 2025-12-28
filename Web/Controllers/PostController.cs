using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers
{
    public class PostController : Controller
    {
        public IActionResult Details(int id)
        {
            // Example: Fetch post from database
            var post = GetPostById(id);
            if (post == null) return NotFound();

            // Decide which partial view to use based on post content
            if (!string.IsNullOrEmpty(post.AudioUrl))
                ViewData["PostPartial"] = "_PostAudio";
            else if (post.ImageUrls != null && post.ImageUrls.Count > 0)
                ViewData["PostPartial"] = "_PostGallery";
            else if (!string.IsNullOrEmpty(post.VideoUrl))
                ViewData["PostPartial"] = "_PostVideo";
            else
                ViewData["PostPartial"] = "_PostStandard";

            // Always include pagination partial
            ViewData["PaginationPartial"] = "_PostPagination";

            return View(post);
        }

        private PostModel? GetPostById(int id)
        {
            // TODO: Replace with actual data access
            return new PostModel
            {
                Id = id,
                Title = "Sample Post",
                Content = "This is a sample post.",
                AudioUrl = null,
                VideoUrl = null,
                ImageUrls = new List<string>()
            };
        }
    }
}
