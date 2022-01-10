using Microsoft.AspNetCore.Http;

namespace StoryboardApp.Models.DTOs
{
    public class CreateHardStoryboardRequest
    {
        public int newHeight { get; set; }
        public int newWidth { get; set; }
        public int paddingLeft { get; set; }
        public int paddingRight { get; set; }
        public int paddingTop { get; set; }
        public int paddingBottom { get; set; }
    }
}