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

        public async Task SaveHardStoryboard(FrameParametersModel request, ImagesTreeModel tree)
        {
            paddingLeft = request.paddingLeft;
            paddingRight = request.paddingRight;
            paddingTop = request.paddingTop;
            paddingBottom = request.paddingBottom;
            
            await tree.ConvertFilesToImage();
            await tree.PrimaryImageTreeMerge();
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
                tree.ScaleImage(similarityValue);
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
                if (similarityValue < 0) similarityValue = 0;
                foreach (var item in tree.ChildTree)
                {
                    if (item.ChildTree == null)
                    {
                        item.ScaleImage(similarityValue);
                        item.Image = await FrameImage(item.Image);
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
    }
}