using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace StoryboardApp.Models.DTOs
{
    public class StoryboardViewModel
    {
        public List<IFormFile> Images { get; set; }
        public int Height { get; set; }
    }
}