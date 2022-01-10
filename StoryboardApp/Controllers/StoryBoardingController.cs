using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoryboardApp.Models;
using StoryboardApp.Models.DTOs;
using StoryboardApp.Services;

namespace StoryboardApp.Controllers
{
    public class StoryBoardingController : Controller
    {
        private readonly EasyStoryboardingService _easyService;
        private readonly MediumStoryboardingService _mediumService;
        private readonly HardStoryboardingService _hardService;
        
        public StoryBoardingController(
            EasyStoryboardingService easyService,
            MediumStoryboardingService mediumService,
            HardStoryboardingService hardService)
        {
            _easyService = easyService;
            _mediumService = mediumService;
            _hardService = hardService;
        }


        [HttpPost]
        public async Task<IActionResult> SaveSimpleStoryboard(StoryboardViewModel request)
        {
            if (request.Images == null || request.Images.Count == 0 || request.Height <= 0)
                return BadRequest();
            ImagesTreeModel tree = new ImagesTreeModel()
            {
                IsRow = true,
                ImageFile = null,
                ChildTree = new List<ImagesTreeModel>()
            };
            foreach (var image in request.Images)
            {
                tree.ChildTree.Add(new ImagesTreeModel()
                {
                    IsRow = false,
                    ImageFile = image
                });
            }
            await _easyService.SaveEasyStoryboard(tree, request.Height);
            return File(tree.ConvertToByteArray(), "image/jpeg", "easy_storyboard_result.jpg");
        }

        [HttpPost]
        public async Task<IActionResult> SaveMediumStoryboard(StoryboardViewModel req)
        {
            ImagesTreeModel tree = new ImagesTreeModel()
            {
                IsRow = true,
                ChildTree = new List<ImagesTreeModel>()
                {
                    new ImagesTreeModel() {IsRow = false, ImageFile = req.Images[0]},
                    new ImagesTreeModel()
                    {
                        IsRow = false, ChildTree = new List<ImagesTreeModel>()
                        {
                            new ImagesTreeModel() {IsRow = true, ImageFile = req.Images[1]},
                            new ImagesTreeModel() {IsRow = true, ImageFile = req.Images[2]},
                            new ImagesTreeModel()
                            {
                                IsRow = true, ChildTree = new List<ImagesTreeModel>()
                                {
                                    new ImagesTreeModel() {IsRow = false, ImageFile = req.Images[3]},
                                    new ImagesTreeModel() {IsRow = false, ImageFile = req.Images[4]}
                                }
                            }
                        }
                    },
                    new ImagesTreeModel() {IsRow = false, ImageFile = req.Images[5]}
                }
            };
            
            await _mediumService.SaveMediumStoryboard(tree, req.Width, req.Height); 
            return File(tree.ConvertToByteArray(), "image/jpeg", "medium_storyboard_result.jpg");
        }
        
        [HttpPost]
        public async Task<IActionResult> SaveHardStoryboard(HardStoryboardViewModel req)
        {
            FrameParametersModel parameters = new FrameParametersModel()
            {
                newHeight = req.Height,
                paddingLeft = req.PaddingLeft,
                paddingRight = req.PaddingRight,
                paddingBottom = req.PaddingBottom,
                paddingTop = req.PaddingTop
            };
            ImagesTreeModel tree = new ImagesTreeModel()
            {
                IsRow = true,
                ChildTree = new List<ImagesTreeModel>()
                {
                    new ImagesTreeModel() {IsRow = false, ImageFile = req.Images[0]},
                    new ImagesTreeModel()
                    {
                        IsRow = false, ChildTree = new List<ImagesTreeModel>()
                        {
                            new ImagesTreeModel() {IsRow = true, ImageFile = req.Images[1]},
                            new ImagesTreeModel() {IsRow = true, ImageFile = req.Images[2]},
                            new ImagesTreeModel()
                            {
                                IsRow = true, ChildTree = new List<ImagesTreeModel>()
                                {
                                    new ImagesTreeModel() {IsRow = false, ImageFile = req.Images[3]},
                                    new ImagesTreeModel() {IsRow = false, ImageFile = req.Images[4]}
                                }
                            }
                        }
                    },
                    new ImagesTreeModel() {IsRow = false, ImageFile = req.Images[5]}
                }
            };
            await _hardService.SaveHardStoryboard(parameters, tree);
            return File(tree.ConvertToByteArray(), "image/jpeg", "hard_storyboard_result.jpg");
        }
    }
}