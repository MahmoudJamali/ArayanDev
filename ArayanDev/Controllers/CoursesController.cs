using System.Diagnostics;
using ArayanDev.Models;
using Microsoft.AspNetCore.Mvc;

namespace ArayanDev.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ILogger<CoursesController> _logger;

        public CoursesController(ILogger<CoursesController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
