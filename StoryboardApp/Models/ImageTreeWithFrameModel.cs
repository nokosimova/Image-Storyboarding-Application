using System.Collections.Generic;
using System.Drawing;
using Microsoft.AspNetCore.Http;

namespace StoryboardApp.Models
{
    public class ImageTreeWithFrameModel
    {
        public bool IsRow { get; set; }
        public IFormFile ImageFile { get; set; }
        public Bitmap OriginalImage { get; set; }
        public Bitmap FramedImage { get; set; }
        public List<ImageTreeWithFrameModel> ChildTree { get; set; } = null;
    }
}