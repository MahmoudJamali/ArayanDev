using Business.Handlers.Users.Queries;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Business.Handlers.Users.Commands;
using System.Globalization;

namespace UI.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "User")]
    public class DashboardController : Controller
    {

        private readonly IMediator _mediator;

        public DashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<IActionResult> Index()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var model = await _mediator.Send(new GetUserDashboardQuery
            {
                UserId = userId
            });

            if (model == null)
                return NotFound();

            return View(model);
        }



        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var profile = await _mediator.Send(new GetUserDashboardQuery
            {
                UserId = userId
            });

            if (profile == null)
                return NotFound();

            var model = new EditProfileViewModel
            {
                UserId = profile.UserId,
                Name = profile.Name,
                Family = profile.Family,
                NationalCode = profile.NationalCode,
                City = profile.City,
                EducationDegree = profile.EducationDegree,
                Major = profile.Major,
                Address = profile.Address,
                Email = profile.Email
            };

            if (profile.BirthDay != default)
            {
                var pc = new PersianCalendar();
                var date = profile.BirthDay.ToDateTime(TimeOnly.MinValue);

                model.BirthYear = pc.GetYear(date);
                model.BirthMonth = pc.GetMonth(date);
                model.BirthDayNumber = pc.GetDayOfMonth(date);
            }

            return View(model);
         }


        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            DateOnly? birthDate = null;

            if (model.BirthYear.HasValue &&
                model.BirthMonth.HasValue &&
                model.BirthDayNumber.HasValue)
            {
                var pc = new PersianCalendar();

                var date = pc.ToDateTime(
                    model.BirthYear.Value,
                    model.BirthMonth.Value,
                    model.BirthDayNumber.Value,
                    0, 0, 0, 0);

                birthDate = DateOnly.FromDateTime(date);
            }

            await _mediator.Send(new UpdateUserProfileCommand
            {
                Model = model,
                BirthDay = birthDate
            });

            return RedirectToAction("Index");
        }




    }
}
