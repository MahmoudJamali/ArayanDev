using System.Diagnostics;
using ArayanDev.Models;
using Business.Handlers.Courses.Queries;
using Business.Handlers.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ArayanDev.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMediator _mediator;

        public HomeController(ILogger<HomeController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }
        public async Task<IActionResult> Index()
        {
            var courses = await _mediator.Send(new GetCoursesQuery());

            //var students = await _mediator.Send(new GetHomeStudentsQuery());

            //ViewBag.Students = students.Students;

            return View(courses);
        }

        public async Task<IActionResult> Students()
        {
            var result = await _mediator.Send(new GetHomeStudentsQuery());
            return PartialView("_StudentsSection", result);
        }



    }
}
