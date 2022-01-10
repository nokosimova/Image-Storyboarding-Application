using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StoryboardApp.Models;
using StoryboardApp.Models.DTOs;
using StoryboardApp.Services;
namespace StoryboardApp.Services
{
    public class HardStoryboardingService
    {
        static int paddingLeft { get; set; }
        static int paddingRight { get; set; }
        static int paddingTop { get; set; }
        static int paddingBottom { get; set; }

        public async Task SaveHardStoryboard(CreateHardStoryboardRequest request, ImagesTreeModel tree)
        {
            paddingLeft = request.paddingLeft;
            paddingRight = request.paddingRight;
            paddingTop = request.paddingTop;
            paddingBottom = request.paddingBottom;

            await tree.PrimaryImageTreeMerge();
            tree.Image.Save("hard_primary_merge.jpg");
            await FrameImageTree(tree, request.newWidth - paddingLeft - paddingRight, 
                                    request.newHeight - paddingTop - paddingBottom);
            tree.Image.Save("hard_framed_merge.jpg");
        }

        public async Task FrameImageTree(ImagesTreeModel tree, int? newWidth, int? newHeight)
        {
            
            if (tree.ChildTree == null)
            {
                decimal similarityValue = tree.IsRow ? (decimal)(newHeight - paddingBottom - paddingTop) / tree.Image.Height 
                                                     : (decimal)(newWidth - paddingLeft - paddingRight) / tree.Image.Width;
                tree.Image = await ScaleImage(tree.Image, similarityValue);
                tree.Image = await FrameImage(tree.Image);
                tree.Image.Save($"scaled_{tree.Image.Height}-{tree.Image.Width}.jpg");
                return;
            }

            if (tree.ChildTree != null)
            {
                var frameCount = tree.ChildTree.Count - 1;
                decimal similarityValue = tree.IsRow ? 
                      (decimal)(newHeight - frameCount* (paddingBottom + paddingTop)) / tree.Image.Height 
                    : (decimal)(newWidth - frameCount* (paddingLeft + paddingRight)) / tree.Image.Width;
                foreach (var item in tree.ChildTree)
                {
                    if (item.ChildTree == null)
                    {
                //        item.Image = await ScaleImage(item.Image, similarityValue);
                        var imageName = $"scaled_{item.ImageFile.FileName}";
                //        item.Image.Save(imageName);
                        item.Image = await FrameImage(item.Image);

                //        imageName = $"framed_{item.ImageFile.FileName}";
                //        item.Image.Save(imageName);
                        continue;
                    }
                    if (item.IsRow)
                        await FrameImageTree(item, null, (int) (item.Image.Height * similarityValue));
                    else
                        await FrameImageTree(item, (int) (item.Image.Width * similarityValue), null);
                }
                if (tree.IsRow)
                    await tree.MergeImageTreeRow();
                else
                    await tree.MergeImageTreeColumn();
            }
        }
        
        
        
        
        public async Task ScaleImageTree(ImagesTreeModel tree, decimal similarityValue)
        {
            if (tree.ChildTree == null)
            {
                await ScaleImage(tree.Image, similarityValue);
                return;
            }
            foreach (var item in tree.ChildTree)
                if (item.ChildTree == null)
                    await ScaleImage(item.Image, similarityValue);
                else
                    await ScaleImageTree(item, similarityValue);
        }

        private async Task<Bitmap> ScaleImage(Bitmap image, decimal similarityValue)
        {
            return new Bitmap(image, new Size(
                (int) (image.Width * similarityValue),
                (int) (image.Height * similarityValue)));
        }
        
        private async Task<Bitmap> FrameImage(Bitmap image)
        {
            Bitmap result = new Bitmap(image.Width + paddingLeft + paddingRight,
                image.Height + paddingTop + paddingBottom);
            //get a graphics object from the image so we can draw on it
            using (System.Drawing.Graphics g = Graphics.FromImage(result))
            {
                //set background color
                g.Clear(System.Drawing.Color.Purple);
                g.DrawImage(image, new System.Drawing.Rectangle(paddingLeft, paddingTop, image.Width, image.Height));
            }
            return result;
        }

        /*    public async Task ResizeImageTreeByHeight(ImagesTreeModel tree, int newHeight)
           {
               if (tree.ChildTree == null)
                   await tree.ResizeImageByHeight(tree.Image, newHeight);
   
               if (tree.ChildTree != null && tree.ChildTree.All(x => x.ChildTree == null))
               {
                   int childImagesCount = tree.ChildTree.Count;
                   decimal similarityValue = (decimal) (newHeight - childImagesCount * (paddingBottom + paddingTop))
                                             / (tree.Image.Height - childImagesCount * (paddingBottom + paddingTop)); 
                   foreach (var item in tree.ChildTree)
                   {
                       var newSubtreeHeight = (int)(item.Image.Height * similarityValue);
                       await item.ResizeImageByHeight(item.Image, newSubtreeHeight);
                   }
                   
               }
   
               if (tree.ChildTree != null && tree.ChildTree.Any(x => x.ChildTree != null))
               {
                   
               }
           }
           
           
        public async Task SaveHardStoryboard(CreateHardStoryboardRequest request, ImageTreeWithFrameModel tree)
           {
               paddingLeft = request.paddingLeft;
               paddingRight = request.paddingRight;
               paddingTop = request.paddingTop;
               paddingBottom = request.paddingBottom;
               
               
               //build primary image tree with frames
               await MergeImage(tree);
               await PrimaryImageTreeMerge(tree);
               tree.FramedImage.Save("hard_result.jpg");
               
               //await FinalImageTreeMerge(tree, request.newHeight);
           }
           
       
           public async Task PrimaryImageTreeMerge(ImageTreeWithFrameModel tree)
           {
               if (tree.ChildTree != null && 
                   tree.ChildTree.Any(x => x.ChildTree != null))
               {
                   foreach (var item in tree.ChildTree)
                   {
                       if (item.ChildTree == null && item.ImageFile != null)
                       {   
                           // create image from file;
                           await ConvertFileToFrameImage(item);
                           continue; 
                       }
                       await PrimaryImageTreeMerge(item);
                   }
               }
   
               if (tree.ChildTree != null && 
                   tree.ChildTree.All(x => x.FramedImage != null || x.ImageFile != null) &&
                   tree.FramedImage == null)
               {
                   foreach (var item in tree.ChildTree)
                       if (item.FramedImage == null)
                           await ConvertFileToFrameImage(item);
                   // merge images:
                   tree.FramedImage = tree.IsRow ? await MergeImageRow(tree) : 
                       await MergeImageColumn(tree);
                   return;
               }
           }
   
           private async Task<Bitmap> MergeImageColumn(ImageTreeWithFrameModel tree)
           {
               int newWidth = tree.ChildTree.Min(x => x.OriginalImage.Height);
               int totalHeight = 0;
               
               //resize all images by height:
               List<Bitmap> resizedImages = new List<Bitmap>();
               foreach (var item in tree.ChildTree)
               {
                   decimal similarityValue = (decimal) newWidth / item.OriginalImage.Width;
                   var newHeight = (int)(item.OriginalImage.Height * similarityValue);
                   item.OriginalImage = new Bitmap(item.OriginalImage, new Size(newWidth, newHeight));
                   item.FramedImage = await FrameImage(item.OriginalImage);
                   totalHeight += item.FramedImage.Height;
                   resizedImages.Add(item.FramedImage);
               }
               
               Bitmap finalImage = new Bitmap(newWidth, totalHeight);
   
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
                           new System.Drawing.Rectangle(0, offset, image.Width, image.Height));
                       offset += image.Height;
                   }
               }
               finalImage.Save("merge_column_result.jpg");
               return finalImage;
           }
           
           private async Task<Bitmap> MergeImageRow(ImageTreeWithFrameModel tree)
           {
               int newHeight = tree.ChildTree.Min(x => x.FramedImage.Height) - paddingBottom - paddingTop;
               int totalWidth = 0;
               
               //resize all images by height:
               List<Bitmap> resizedImages = new List<Bitmap>();
               foreach (var item in tree.ChildTree)
               {
                   decimal similarityValue = (decimal) newHeight / (item.FramedImage.Height - paddingBottom - paddingTop);
                   var newWidth = (int)((item.OriginalImage.Width - paddingLeft - paddingRight) * similarityValue);
                   item.OriginalImage = new Bitmap(item.OriginalImage, new Size(newWidth, newHeight));
                   item.FramedImage = await FrameImage(item.OriginalImage);
                   totalWidth += item.FramedImage.Width;
                   resizedImages.Add(item.FramedImage);
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
   
           private async Task<Bitmap> FrameImage(Bitmap image)
           {
               Bitmap result = new Bitmap(image.Width + paddingLeft + paddingRight,
                   image.Height + paddingTop + paddingBottom);
               //get a graphics object from the image so we can draw on it
               using (System.Drawing.Graphics g = Graphics.FromImage(result))
               {
                   //set background color
                   g.Clear(System.Drawing.Color.Purple);
                   g.DrawImage(image, new System.Drawing.Rectangle(paddingLeft, paddingTop, image.Width, image.Height));
               }
   
               return result;
           }
           
           public async Task FinalImageTreeMerge(ImageTreeWithFrameModel tree, int newHeight)
           {
               
           }
           
           public async Task ConvertFileToFrameImage(ImageTreeWithFrameModel tree)
           {
               await using var stream = new MemoryStream();
               await tree.ImageFile.CopyToAsync(stream);
               using var img = Image.FromStream(stream);
               
               var newImage = new Bitmap(img.Width, img.Height);
               var newImageWithFrame = new Bitmap(img.Width + paddingLeft + paddingRight, 
                                           img.Height + paddingTop + paddingBottom);
               using Graphics g1 = Graphics.FromImage(newImage);
               using Graphics g2 = Graphics.FromImage(newImageWithFrame);
   
               g2.Clear(Color.Purple);
               g1.DrawImage(img, 0, 0, img.Width, img.Height);
               g2.DrawImage(img, paddingLeft, paddingTop, img.Width, img.Height);
   
               tree.OriginalImage = newImage;
               tree.FramedImage = newImageWithFrame;
           }
   
   
   
           private async Task ResizeImageByHeight(ImageTreeWithFrameModel tree, int newHeight)
           {
               if (tree.ChildTree == null)
               {
                   // resize simple image
               }
               else
               {
                   //resize composite image:
               }
           }
           
           private async Task ResizeImageByWidth(ImageTreeWithFrameModel tree, int newWidth)
           {
               if (tree.ChildTree == null)
               {
                   // resize simple image
               }
               else
               {
                   //resize composite image:
               }
               
           }*/
        
    }
}