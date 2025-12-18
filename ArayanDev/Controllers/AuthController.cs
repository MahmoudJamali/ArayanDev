using System.Diagnostics;
using ArayanDev.Models;
using ArayanDev.ViewModels;
using ArayanDev.Business.Handlers.Users.Commands;
using Business.Services.Authentication;
using MediatR;
using Microsoft.AspNetCore.Mvc;



namespace ArayanDev.Controllers
{
    [Route("auth")]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService, IMediator mediator) : base(mediator)
        {
            _authService = authService;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View(new UserLoginCommand());
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginCommand command)
        {
            if (!ModelState.IsValid)
                return View(command);

            var result = await _mediator.Send(command);
       
            return View("");
        }
        [HttpPost("api-login")]
        public async Task<IActionResult> ApiLogin([FromBody] UserLoginCommand command)
        {
            if (!ModelState.IsValid)
                return BadRequest("اطلاعات نامعتبر است.");

            var result = await _mediator.Send(command);

            return Ok(new
            {
                accessToken = result.accessToken,
                refreshToken = result.refreshToken
            });
        }
        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterCommand command)
        {
            if (!ModelState.IsValid)
            {
                SetErrorMessage("لطفاً اطلاعات را به‌درستی وارد کنید.");
                return View(command);
            }

            await _mediator.Send(command);
            SetSuccessMessage("ثبت‌نام با موفقیت انجام شد. لطفاً وارد شوید.");
            return RedirectToAction("Login");
        }
        [HttpGet("reset-password")]
        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetUserPasswordCommand command)
        {
            if (!ModelState.IsValid)
            {
                SetErrorMessage("لطفاً اطلاعات را به‌درستی وارد کنید.");
                return View(command);
            }

            try
            {
                await _mediator.Send(command);
                SetSuccessMessage("رمز عبور با موفقیت تغییر کرد. لطفاً وارد شوید.");
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                SetErrorMessage(ex.Message);
                return View(command);
            }
        }
        [HttpGet("change-password")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordCommand command)
        {
            if (!ModelState.IsValid)
            {
                SetErrorMessage("لطفاً اطلاعات را به‌درستی وارد کنید.");
                return View(command);
            }

            try
            {
                await _mediator.Send(command);
                SetSuccessMessage("رمز عبور با موفقیت تغییر کرد.");
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                SetErrorMessage(ex.Message);
                return View(command);
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var (accessToken, newRefreshToken) = await _authService.RefreshTokenAsync(request.RefreshToken);
                return Json(new
                {
                    accessToken,
                    refreshToken = newRefreshToken
                });
            }
            catch (UnauthorizedAccessException)
            {
                return JsonError("توکن معتبر نیست یا منقضی شده.", 401);
            }
        }
    }
}
