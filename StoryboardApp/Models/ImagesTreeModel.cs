using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace StoryboardApp.Models
{
    public class ImagesTreeModel
    {
        public bool IsRow { get; set; }
        public IFormFile ImageFile { get; set; }
        public Bitmap Image { get; set; }
        public Bitmap OriginalImage { get; set; }
        public List<ImagesTreeModel> ChildTree { get; set; } = null;
        
        //this method create primary image merging starting from inner images using recursion:
        public async Task PrimaryImageTreeMerge()
        {
            ConvertFilesToImage();
            if (ChildTree != null && ChildTree.Any(x => x.ChildTree != null))
            {
                foreach (var item in ChildTree)
                {
                    if (item.ChildTree != null)
                        await item.PrimaryImageTreeMerge();
                }
            }

            if (ChildTree != null && ChildTree.All(x => x.Image != null || x.ImageFile != null))
            {
                // merge images:
                if (IsRow)
                    await this.MergeImageTreeRow();
                else
                    await this.MergeImageTreeColumn();
            }
        }

        public async Task MergeImageTreeRow()
        {
            int newHeight =  ChildTree.Max(x => x.Image.Height);  
            
            //resize all images by newHeight:
            foreach (var item in ChildTree)
                await item.ResizeImageByHeight(newHeight);
            
            //connect all images into one row
            Bitmap finalImage = new Bitmap(ChildTree.Sum(x => x.Image.Width), newHeight);
            using var graphics = Graphics.FromImage(finalImage);
            graphics.Clear(Color.Red);
            int xPosition = 0;
            foreach (var item in ChildTree)
            {
                graphics.DrawImage(item.Image, new Rectangle(xPosition, 0, item.Image.Width, item.Image.Height));
                xPosition += item.Image.Width;
            }
           // finalImage.Save("merge_row_result.jpg");
            this.Image = finalImage;
        }

        public async Task MergeImageTreeColumn()
        {
            int newWidth = ChildTree.Max(x => x.Image.Width);
            
            //resize all images by width:
            foreach (var item in ChildTree)
                await item.ResizeImageByWidth(newWidth);
            //connect all images into one column
            Bitmap finalImage = new Bitmap(newWidth, ChildTree.Sum(x => x.Image.Height));
            using var graphics = Graphics.FromImage(finalImage);
            graphics.Clear(Color.Cyan);
            int yPosition = 0;
            foreach (var item in ChildTree)
            {
                graphics.DrawImage(item.Image, new Rectangle(0, yPosition, item.Image.Width, item.Image.Height));
                yPosition += item.Image.Height;
            }
         //   finalImage.Save("merge_column_result.jpg");
            this.Image = finalImage;
        }
        
        public async Task ResizeImageByHeight(int newHeight)
        {
            decimal similarityValue = (decimal)newHeight / Image.Height;
            var newWidth = (int)(Image.Width * similarityValue);
            Image = new Bitmap(Image, new Size(newWidth, newHeight));
        }
        
        public async Task ResizeImageByWidth(int newWidth)
        {
            decimal similarityValue = (decimal)newWidth / Image.Width;
            var newHeight = (int)(Image.Height * similarityValue);
            Image = new Bitmap(Image, new Size(newWidth, newHeight));
        }

        public async Task ConvertFilesToImage()
        {
            if (ChildTree != null)
                foreach (var item in ChildTree)
                    item.ConvertFilesToImage();
            if (ImageFile == null) return;
            
            using var stream = new MemoryStream();
            ImageFile.CopyTo(stream);
            using var fileImage = System.Drawing.Image.FromStream(stream);
            var newImage = new Bitmap(fileImage.Width, fileImage.Height);
            using var graphics = Graphics.FromImage(newImage);
            graphics.DrawImage(fileImage, 0, 0, fileImage.Width, fileImage.Height);
            this.Image = newImage;
            OriginalImage = newImage;
        }

        public void ScaleImage(decimal similarityValue)
        {
            Image = new Bitmap(Image, new Size(
                (int) (Image.Width * similarityValue),
                (int) (Image.Height * similarityValue)));
        }
    }
}