using System.Security.Claims;
using ArayanDev.Models;
using Business.Handlers.Courses.Commands;
using Business.Handlers.Courses.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArayanDev.Controllers
{
    public class CourseController : Controller
    {
        private readonly IMediator _mediator;

        public CourseController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // کاربر مهمان آزادانه وارد صفحه جزئیات می‌شود
        public async Task<IActionResult> Details(Guid id)
        {
            var userId = User.Identity.IsAuthenticated
                ? Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))
                : Guid.Empty;

            var course = await _mediator.Send(new GetCourseByIdQuery { CourseId = id });
            if (course == null)
                return NotFound();

            bool isEnrolled = false;

            if (userId != Guid.Empty)
            {
                isEnrolled = await _mediator.Send(new CheckEnrollmentQuery
                {
                    UserId = userId,
                    CourseId = id
                });
            }

            var vm = new CourseDetailsViewModel
            {
                Course = course,
                IsAuthenticated = userId != Guid.Empty,
                IsEnrolled = isEnrolled
            };

            return View(vm);
        }

        // ثبت‌نام فقط هنگام لاگین + تکمیل پروفایل
        [Authorize] // اول باید لاگین باشد
        [Authorize(Policy = "CompleteProfile")] // بعد پروفایل کامل باشد
        [HttpGet]
        public async Task<IActionResult> Enroll(Guid courseId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var result = await _mediator.Send(new EnrollInCourseCommand
            {
                UserId = userId,
                CourseId = courseId
            });

            if (!result)
            {
                TempData["Error"] = "ثبت‌نام انجام نشد.";
                return RedirectToAction("Details", new { id = courseId });
            }

            TempData["Success"] = "ثبت‌نام با موفقیت انجام شد.";
            return RedirectToAction("MyCourses");
        }
    }
}
