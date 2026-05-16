using MediatR;
using Microsoft.AspNetCore.Mvc;
using Business.Handlers.Authentication.Commands;
using Business.Handlers.UserProfiles.Commands;
using System.Security.Claims;
using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using DataAccess.Abstract;

namespace UI.Controllers
{
    public class AuthController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IOtpRepository _otpRepository;

        public AuthController(IMediator mediator, IOtpRepository otpRepository)
        {
            _mediator = mediator;
            _otpRepository = otpRepository;
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
        public async Task<IActionResult> VerifyOtp(string phoneNumber, string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;

            var otp = await _otpRepository.GetLastOtpByPhoneAsync(phoneNumber);

            if (otp != null)
                ViewBag.ExpireAt = otp.ExpireAt;

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

                var otp = await _otpRepository.GetLastOtpByPhoneAsync(phoneNumber);

                if (otp != null)
                    ViewBag.ExpireAt = otp.ExpireAt;

                return View(model: phoneNumber);
            }

            if (result == OtpVerifyResult.Expired)
            {
                ModelState.AddModelError("", "کد منقضی شده است.");

                var otp = await _otpRepository.GetLastOtpByPhoneAsync(phoneNumber);

                if (otp != null)
                    ViewBag.ExpireAt = otp.ExpireAt;

                return View(model: phoneNumber);
            }

            await _mediator.Send(new LoginCommand
            {
                PhoneNumber = phoneNumber,
                Otp = otpCode
            });

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

