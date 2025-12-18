using System.Diagnostics;
using ArayanDev.Models;
using Microsoft.AspNetCore.Mvc;
using MediatR;



namespace ArayanDev.Controllers
{
    public abstract class BaseController : Controller
    {

        protected readonly IMediator _mediator;

    protected BaseController(IMediator mediator)
    {
        _mediator = mediator;
    }
        protected void SetSuccessMessage(string message)
        {
            TempData["SuccessMessage"] = message;
        }

        protected void SetErrorMessage(string message)
        {
            TempData["ErrorMessage"] = message;
        }

        protected IActionResult RedirectToHome()
        {
            return RedirectToAction("Index", "Home");
        }

        protected IActionResult RedirectWithSuccess(string action, string controller, string message)
        {
            SetSuccessMessage(message);
            return RedirectToAction(action, controller);
        }

        protected IActionResult RedirectWithError(string action, string controller, string message)
        {
            SetErrorMessage(message);
            return RedirectToAction(action, controller);
        }

        protected JsonResult JsonSuccess(object? data = null, string? message = null)
        {
            return Json(new
            {
                success = true,
                message,
                data
            });
        }

        protected JsonResult JsonError(string message, int statusCode = 400)
        {
            Response.StatusCode = statusCode;
            return Json(new
            {
                success = false,
                message
            });
        }
    }
}
