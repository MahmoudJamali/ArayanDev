using MediatR;
using Microsoft.AspNetCore.Mvc;
using Business.Handlers.Authentication.Commands;

namespace UI.Controllers
{
    public class AuthController : Controller
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> SendOtp(string phoneNumber)
        {
            await _mediator.Send(new SendOtpCommand { PhoneNumber = phoneNumber });
            return RedirectToAction("VerifyOtp", new { phoneNumber });
        }

        [HttpGet]
        public IActionResult VerifyOtp(string phoneNumber) => View(model: phoneNumber);

        [HttpPost]
        public async Task<IActionResult> VerifyOtp(string phoneNumber, string otpCode)
        {
            var ok = await _mediator.Send(new VerifyOtpCommand
            {
                PhoneNumber = phoneNumber,
                OtpCode = otpCode
            });

            if (!ok)
            {
                ModelState.AddModelError("", "کد وارد شده نادرست است.");
                return View(model: phoneNumber);
            }

            await _mediator.Send(new LoginCommand { PhoneNumber = phoneNumber });

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _mediator.Send(new LogoutCommand());
            return RedirectToAction("Login");
        }
    }
}

