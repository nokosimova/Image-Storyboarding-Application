using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StoryboardApp.Models;

namespace StoryboardApp.Services
{
    public class MediumStoryboardingService
    {
        public async Task SaveMediumStoryboard(ImagesTreeModel tree, int newWidth, int newHeight)
        {
            await tree.ConvertFilesToImage();
            await tree.PrimaryImageTreeMerge();

            var similarityValue = tree.IsRow ? (decimal)newHeight / tree.Image.Height : 
                                                     (decimal)newWidth / tree.Image.Width; 
            tree.ScaleImage(similarityValue);
        }
    }
}