using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoryboardApp.Models;
using StoryboardApp.Services;

namespace StoryboardApp.Controllers
{
    public class StoryBoardingController : Controller
    {
        private readonly EasyStoryboardingService _easyService;
        private readonly MediumStoryboardingService _mediumService;

        public StoryBoardingController(
            EasyStoryboardingService easyService,
            MediumStoryboardingService mediumService)
        {
            _easyService = easyService;
            _mediumService = mediumService;
        }


        [HttpPost]
        public async Task SaveSimpleStoryboard(List<IFormFile> Images, int Height)
        {
            var resultImage = await _easyService.SaveEasyStoryboard(Images, Height);
            resultImage.Save("result_easy_storyboarding.jpg");
        }

        [HttpPost]
        public async Task SaveMediumStoryboard(List<IFormFile> Images, int Height)
        {
            // you should load at least 4 images:
            ImagesTreeModel tree1 = new ImagesTreeModel()
            {
                IsRow = true,
                ChildTree = new List<ImagesTreeModel>()
                {
                    new ImagesTreeModel() {IsRow = false, ImageFile = Images[0]},
                    new ImagesTreeModel()
                    {
                        IsRow = false, ChildTree = new List<ImagesTreeModel>()
                        {
                            new ImagesTreeModel() {IsRow = true, ImageFile = Images[1]},
                            new ImagesTreeModel() {IsRow = true, ImageFile = Images[2]}
                        }
                    },
                    new ImagesTreeModel() {IsRow = false, ImageFile = Images[3]}
                }
            };
            
            ImagesTreeModel tree2 = new ImagesTreeModel()
            {
                IsRow = true,
                ChildTree = new List<ImagesTreeModel>()
                {
                    new ImagesTreeModel() {IsRow = false, ImageFile = Images[0]},
                    new ImagesTreeModel()
                    {
                        IsRow = false, ChildTree = new List<ImagesTreeModel>()
                        {
                            new ImagesTreeModel() {IsRow = true, ImageFile = Images[1]},
                            new ImagesTreeModel() {IsRow = true, ImageFile = Images[2]}
                        }
                    }
                }
            };
            
            ImagesTreeModel tree3 = new ImagesTreeModel()
            {
                IsRow = true,
                ChildTree = new List<ImagesTreeModel>()
                {
                    new ImagesTreeModel() {IsRow = false, ImageFile = Images[0]},
                    new ImagesTreeModel()
                    {
                        IsRow = false, ChildTree = new List<ImagesTreeModel>()
                        {
                            new ImagesTreeModel() {IsRow = true, ImageFile = Images[1]},
                            new ImagesTreeModel() {IsRow = true, ImageFile = Images[2]},
                            new ImagesTreeModel()
                            {
                                IsRow = true, ChildTree = new List<ImagesTreeModel>()
                                {
                                    new ImagesTreeModel() {IsRow = false, ImageFile = Images[3]},
                                    new ImagesTreeModel() {IsRow = false, ImageFile = Images[4]}
                                }
                            }
                        }
                    },
                    new ImagesTreeModel() {IsRow = false, ImageFile = Images[5]}
                }
            };
            await _mediumService.MergeImage(tree2);
            var resultImage = _easyService.ScaleImageByHeight(tree2.Image, Height);
            resultImage.Save("result_medium_storyboarding2.jpg");
            
            await _mediumService.MergeImage(tree3);
            resultImage = _easyService.ScaleImageByHeight(tree3.Image, Height);
            resultImage.Save("result_medium_storyboarding3.jpg");
        }

    }
}