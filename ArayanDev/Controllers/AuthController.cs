using MediatR;
using Microsoft.AspNetCore.Mvc;
using Business.Handlers.Authentication.Commands;
using Business.Handlers.UserProfiles.Commands;
using System.Security.Claims;
using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using DataAccess.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Entities.Concrete;

namespace UI.Controllers
{
    public class AuthController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IAuthService _authService;
        public AuthController(IMediator mediator, IAuthService authService)
        {
            _authService = authService;
            _mediator = mediator;
        }
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
            var result = await _mediator.Send(new VerifyOtpCommand
            {
                PhoneNumber = phoneNumber,
                OtpCode = otpCode
            });

            if (result == OtpVerifyResult.InvalidCode)
            {
                ModelState.AddModelError("", "کد وارد شده اشتباه است.");
                ViewBag.ReturnUrl = returnUrl;
                return View(model: phoneNumber);
            }

            if (result == OtpVerifyResult.Expired)
            {
                ModelState.AddModelError("", "کد منقضی شده است.");
                return View(model: phoneNumber);
            }

            // ✅ اینجا کوکی ساخته می‌شود
            var loginSuccess = await _authService.LoginWithOtpAsync(phoneNumber,  HttpContext);

            if (!loginSuccess)
            {
                ModelState.AddModelError("", "خطا در ورود.");
                return View(model: phoneNumber);
            }

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
        [Authorize]
        [HttpGet]
        public IActionResult ProfileIncomplete(string? returnUrl = null)
        {
            return RedirectToAction("CompleteProfile", "Auth", new { returnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> SubmitCompleteProfile(UserProfile model , string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(model);

            // ۱. گرفتن اطلاعات فعلی کاربر از کوکی قبل از SignOut
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var phoneNumber = User.FindFirstValue(ClaimTypes.MobilePhone);
            var role = User.FindFirstValue(ClaimTypes.Role);

            // ۲. ذخیره اطلاعات در دیتابیس (مثلاً با Mediator یا Service)
            await _authService.UpdateProfileAsync(Guid.Parse(userId), model);

            // ۳. خروج کاربر (پاک کردن کوکی قدیمی)
            await HttpContext.SignOutAsync("MyCookieAuth");

            // ۴. صدور کوکی جدید با مقدار جدید "true"
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, userId),
        new Claim(ClaimTypes.MobilePhone, phoneNumber),
        new Claim(ClaimTypes.Role, role),
        new Claim("IsPhoneConfirmed", "True"),
        new Claim("ProfileCompleted", "true")
    };

            var identity = new ClaimsIdentity(claims, "MyCookieAuth");
            await HttpContext.SignInAsync("MyCookieAuth", new ClaimsPrincipal(identity));

            TempData["Success"] = "پروفایل شما با موفقیت تکمیل شد.";
            if (string.IsNullOrWhiteSpace(returnUrl))
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

