using System.Collections.Generic;

namespace Web.Models
{
    public class PostModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? AudioUrl { get; set; }
        public string? VideoUrl { get; set; }
        public List<string> ImageUrls { get; set; } = new();
        public int PageNumber { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        // Add more properties as needed
    }
}
