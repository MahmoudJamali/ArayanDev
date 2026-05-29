using Business.Handlers.EnrollCourses.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class DashboardController : Controller
    {

        private readonly IMediator _mediator;

        public DashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<IActionResult> Index(Guid? courseId)
        {
            var enrollments = await _mediator.Send(new GetEnrollmentsQuery
            {
                CourseId = courseId
            });

            return View(enrollments);
        }
    }
}
