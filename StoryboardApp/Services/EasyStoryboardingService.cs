using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StoryboardApp.Models;

namespace StoryboardApp.Services
{
    public class EasyStoryboardingService
    {
        public async Task<Bitmap> SaveEasyStoryboard(List<IFormFile> Images, int newHeight)
        {
            ImagesTreeModel tree = new ImagesTreeModel()
            {
                IsRow = true,
                ImageFile = null,
                ChildTree = new List<ImagesTreeModel>()
            };
            foreach (var image in Images)
            {
                tree.ChildTree.Add(new ImagesTreeModel()
                 {
                     IsRow = false,
                     ImageFile = image
                 });
            }

            await tree.ConvertFilesToImage();
            await tree.MergeImageTreeRow();
            tree.ScaleImage((decimal)newHeight / tree.Image.Height);
            return tree.Image;
        }
        public Bitmap ScaleImageByHeight(Image image, int newHeight)
        {
            var currentHeight = image.Height;
            var currentWidth = image.Width;
            decimal similarityValue = (decimal)newHeight/currentHeight;
            var newWidth = (int)(currentWidth * similarityValue);
            Bitmap newImage = new Bitmap(newWidth, newHeight);
            using (Graphics g = Graphics.FromImage(newImage))
            {
                g.DrawImage(image, 0, 0, newWidth, newHeight);
            }
            image.Dispose();
            return newImage;
        }
    }
}