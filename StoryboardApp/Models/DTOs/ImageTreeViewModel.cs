using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace StoryboardApp.Models.DTOs
{
    public class ImageTreeViewModel
    {
        public bool IsRow { get; set; }
        public IFormFile ImageFile { get; set; }
        public List<ImageTreeViewModel> ChildTree { get; set; } = null;
    }
}