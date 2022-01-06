using System.Collections.Generic;
using System.Drawing;
using Microsoft.AspNetCore.Http;

namespace StoryboardApp.Models
{
    public class ImagesTreeModel
    {
        public bool IsRow { get; set; }
        public IFormFile ImageFile { get; set; }
        public Bitmap Image { get; set; }
        public List<ImagesTreeModel> ChildTree { get; set; } = null;
    }
}