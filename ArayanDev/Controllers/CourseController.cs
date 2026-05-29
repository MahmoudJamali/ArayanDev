using System.Security.Claims;
using ArayanDev.Models;
using Business.Handlers.Courses.Commands;
using Business.Handlers.Courses.Queries;
using Entities.Concrete;
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

        [Authorize]
        [Authorize(Policy = "CompleteProfile")]
        [HttpGet]
        public async Task<IActionResult> Enroll(Guid courseId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // دریافت مستقیم موجودیت Course از مدیااتور
            Course? course = await _mediator.Send(new GetCourseByIdQuery { CourseId = courseId });
            if (course == null)
            {
                return NotFound();
            }

            // بررسی اینکه آیا کاربر از قبل ثبت‌نام کرده است یا خیر
            var isEnrolled = await _mediator.Send(new CheckEnrollmentQuery
            {
                UserId = userId,
                CourseId = courseId
            });

            if (isEnrolled)
            {
                TempData["Info"] = "شما قبلاً در این دوره ثبت‌نام کرده‌اید.";
                return RedirectToAction("Details", new { id = courseId });
            }

            // ارسال مستقیم مدل Course به ویوی تایید ثبت‌نام
            return View("Enroll", course);
        }

        [Authorize]
        [Authorize(Policy = "CompleteProfile")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmEnroll(Guid courseId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // بررسی مجدد جهت جلوگیری از ثبت‌نام تکراری به هر دلیلی
            var isEnrolled = await _mediator.Send(new CheckEnrollmentQuery
            {
                UserId = userId,
                CourseId = courseId
            });

            if (isEnrolled)
            {
                TempData["Info"] = "شما قبلاً در این دوره ثبت‌نام کرده‌اید.";
                return RedirectToAction("Details", new { id = courseId });
            }

            // اجرای فرمان ثبت‌نام نهایی
            var result = await _mediator.Send(new EnrollInCourseCommand
            {
                UserId = userId,
                CourseId = courseId
            });

            if (!result)
            {
                TempData["Error"] = "ثبت‌نام با خطا مواجه شد. لطفاً مجدداً تلاش کنید.";
                return RedirectToAction("Details", new { id = courseId });
            }

            TempData["Success"] = "ثبت‌نام شما با موفقیت تکمیل شد.";
            return RedirectToAction("Details", new { id = courseId });
        }


    }
}
