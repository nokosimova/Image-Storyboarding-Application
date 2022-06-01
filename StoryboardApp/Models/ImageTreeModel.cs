using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace StoryboardApp.Models
{
    public class ImageTreeModel
    {
        public bool IsRow { get; set; }
        public IFormFile ImageFile { get; set; }
        public Bitmap Image { get; set; }
        public List<ImageTreeModel> ChildTree { get; set; } = null;
    }
}