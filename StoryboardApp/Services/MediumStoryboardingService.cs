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
        public async Task MergeImage(ImagesTreeModel tree)
        {
            if (tree.ChildTree != null && tree.ChildTree.Any(x => x.ChildTree != null))
            {
                foreach (var item in tree.ChildTree)
                {
                    if (item.ChildTree == null && item.ImageFile != null)
                    {   
                        // create image from file;
                        item.Image = await ConvertFileToImage(item.ImageFile);
                        item.ChildTree = null;
                        continue; 
                    }
                    await MergeImage(item);
                }
            }

            if (tree.ChildTree != null && tree.ChildTree.All(x => x.ChildTree == null))
            {
                foreach (var item in tree.ChildTree)
                    if (item.Image == null)
                        item.Image = await ConvertFileToImage(item.ImageFile);
                var childImages = tree.ChildTree.Select(x => x.Image).ToList();
                // merge images:
                tree.Image = tree.IsRow ? await MergeImageRow(childImages) : 
                                          await MergeImageColumn(childImages);
                tree.ChildTree = null;
                return;
            }
        }

        public async Task<Bitmap> MergeImageRow(List<Bitmap> images)
        {
            int totalWidth = 0;
            int newHeight = images.Min(x => x.Height);
            //resize all images by height:
            List<Bitmap> resizedImages = new List<Bitmap>();
            foreach (var img in images)
            {
                decimal similarityValue = (decimal)newHeight / img.Height;
                var newWidth = (int)(img.Width * similarityValue);
                totalWidth += newWidth;
                resizedImages.Add(new Bitmap(img,new Size(newWidth,newHeight)));
            }
            
            Bitmap finalImage = new Bitmap(totalWidth, newHeight);

            //get a graphics object from the image so we can draw on it
            using (System.Drawing.Graphics g = Graphics.FromImage(finalImage))
            {
                //set background color
                g.Clear(System.Drawing.Color.Chartreuse);

                //go through each image and draw it on the final image
                int offset = 0;
                foreach (System.Drawing.Bitmap image in resizedImages)
                {
                    g.DrawImage(image,
                        new System.Drawing.Rectangle(offset, 0, image.Width, image.Height));
                    offset += image.Width;
                }
            }
            finalImage.Save("merge_row_result.jpg");
            return finalImage;
        }
        
        public async Task<Bitmap> MergeImageColumn(List<Bitmap> images)
        {
            int totalHeight = 0;
            int newWidth = images.Min(x => x.Width);
            //resize all images by width:
            List<Bitmap> resizedImages = new List<Bitmap>();
            foreach (var img in images)
            {
                decimal similarityValue = (decimal)newWidth / img.Width;
                var newHeight = (int)(img.Height * similarityValue);
                totalHeight += newHeight;
                resizedImages.Add(new Bitmap(img,new Size(newWidth,newHeight)));
            }
            
            Bitmap finalImage = new System.Drawing.Bitmap(newWidth, totalHeight);

            //get a graphics object from the image so we can draw on it
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(finalImage))
            {
                //set background color
                g.Clear(System.Drawing.Color.HotPink);

                //go through each image and draw it on the final image
                int offset = 0;
                foreach (System.Drawing.Bitmap image in resizedImages)
                {
                    g.DrawImage(image,
                        new System.Drawing.Rectangle(0, offset, image.Width, image.Height));
                    offset += image.Height;
                }
            }
            finalImage.Save("merge_column_result.jpg");
            return finalImage;
        }

        public async Task<Bitmap> ConvertFileToImage(IFormFile file)
        {
            await using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            using var img = Image.FromStream(stream);
            
            var newImage = new Bitmap(img.Width, img.Height);
            using Graphics g = Graphics.FromImage(newImage);
            g.DrawImage(img, 0, 0, img.Width, img.Height);
            return newImage;
        }
        
        /*public async Task<Bitmap> ScaleImage(ImagesTreeModel tree, int width, int height)
        {
            Bitmap newImage;
            if (width > 0 && height > 0)
            {
                // in this case scale by w and h:
                if (tree.Image != null)
                {
                    //storyboard single image and return it:
                    newImage = new Bitmap(width, height);
                    await using var stream = new MemoryStream();
                    await tree.Image.CopyToAsync(stream);
                    using var img = System.Drawing.Image.FromStream(stream);
                    using (Graphics g = Graphics.FromImage(newImage))
                    {
                        g.DrawImage(img, 0, 0, width, height);
                    }
                    return newImage;
                }
                else
                {
                    foreach (var item in tree.ChildTree)
                    {
                        //if isRow, then scale by height, else scale by width
                    }                    
                }
            }
            else
            {
                // в этом случае нужно раскадрировать только по длине (ширине), другой параметр не имеет значения
                //обычно этот случай бывает только в первом уровне дерева изображений
            }
            newImage = new Bitmap(width, height);
            return newImage;
        }*/
    }
}