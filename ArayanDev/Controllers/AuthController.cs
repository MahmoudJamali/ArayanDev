using MediatR;
using Microsoft.AspNetCore.Mvc;
using Business.Handlers.Authentication.Commands;
using Business.Handlers.UserProfiles.Commands;
using System.Security.Claims;

namespace UI.Controllers
{
    public class AuthController : Controller
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl; // ذخیره آدرس بازگشت
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendOtp(string phoneNumber, string? returnUrl = null)
        {
            await _mediator.Send(new SendOtpCommand { PhoneNumber = phoneNumber });
            // فرستادن returnUrl به مرحله بعد
            return RedirectToAction("VerifyOtp", new { phoneNumber, returnUrl });
        }

        [HttpGet]
        public IActionResult VerifyOtp(string phoneNumber, string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(model: phoneNumber);
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOtp(string phoneNumber, string otpCode, string? returnUrl = null)
        {
            var ok = await _mediator.Send(new VerifyOtpCommand { PhoneNumber = phoneNumber, OtpCode = otpCode });

            if (!ok)
            {
                ModelState.AddModelError("", "کد وارد شده نادرست یا منقضی شده است.");
                ViewBag.ReturnUrl = returnUrl;
                return View(model: phoneNumber);
            }

            // انجام لاگین و ایجاد کوکی
            await _mediator.Send(new LoginCommand { PhoneNumber = phoneNumber, Otp = otpCode });

            // تصمیم‌گیری هوشمند:
            // اگر returnUrl خالی بود، برو به صفحه اصلی
            // اگر returnUrl داشت، برو به همان آدرس (مثلاً صفحه Enroll دوره)
            if (string.IsNullOrEmpty(returnUrl))
                return RedirectToAction("Index", "Home");

            return LocalRedirect(returnUrl);
        }

        [HttpGet]
        public IActionResult CompleteProfile(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CompleteProfile(CompleteProfileModel model, string? returnUrl = null)
        {
            // نکته: مطمئن شو در LoginCommandHandler نام کلیم را "UserId" گذاشته‌ای
            // پیشنهاد استاندارد: ClaimTypes.NameIdentifier
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var result = await _mediator.Send(new CompleteProfileCommand { UserId = userId, Model = model });

            if (!result)
            {
                ModelState.AddModelError("", "ثبت اطلاعات با خطا مواجه شد.");
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            // بعد از تکمیل پروفایل، برگرد به جایی که بودی (مثلاً همان دکمه ثبت نام)
            if (string.IsNullOrEmpty(returnUrl))
                return RedirectToAction("Index", "Home");

            return LocalRedirect(returnUrl);
        }

        public async Task<IActionResult> Logout()
        {
            await _mediator.Send(new LogoutCommand());
            return RedirectToAction("Index", "Home");
        }
    }

}

