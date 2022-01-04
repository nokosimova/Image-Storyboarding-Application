using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
namespace StoryboardApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult EasyStoryboarding()
        {
            return View();
        }
        
        public IActionResult MediumStoryboarding()
        {
            return View();
        }
        
        public IActionResult HardStoryboarding()
        {
            return View();
        }
        
    }
}