using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace StoryboardApp.Services
{
    public class EasyStoryboardingService
    {
        public async Task<Bitmap> SaveEasyStoryboard(List<IFormFile> Images, int Height)
        {
            int totalWidth = 0;
            List<Bitmap> newImages = new List<Bitmap>();
            foreach (var image in Images)
            {
                using (var stream = new MemoryStream())
                {
                    await image.CopyToAsync(stream);
                    using (var img = System.Drawing.Image.FromStream(stream))
                    {
                        Bitmap newImage = ScaleImageByHeight(img, Height);
                        totalWidth += newImage.Width;
                        newImages.Add(newImage);
                    }
                }
            }
            
            //create a bitmap to hold the combined image
            Bitmap finalImage = new System.Drawing.Bitmap(totalWidth, Height);

            //get a graphics object from the image so we can draw on it
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(finalImage))
            {
                //set background color
                g.Clear(System.Drawing.Color.Black);

                //go through each image and draw it on the final image
                int offset = 0;
                foreach (System.Drawing.Bitmap image in newImages)
                {
                    g.DrawImage(image,
                        new System.Drawing.Rectangle(offset, 0, image.Width, image.Height));
                    offset += image.Width;
                }
            }
            return finalImage;
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