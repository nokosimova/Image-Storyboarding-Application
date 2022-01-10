using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace StoryboardApp.Models.DTOs
{
    public class HardStoryboardViewModel
    {
        public List<IFormFile> Images { get; set; }
        public int Height { get; set; }
        public int PaddingLeft { get; set; }
        public int PaddingRight { get; set; }
        public int PaddingTop { get; set; }
        public int PaddingBottom { get; set; }
    }
}