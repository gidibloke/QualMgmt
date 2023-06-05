using Application.Core;
using Application.Interfaces;
using Application.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Web.Controllers
{
    [AllowAnonymous]
    public class AccountController : BaseController
    {
        private readonly IAccount _account;

        public AccountController(IAccount account)
        {
            _account = account;
        }
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if(!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await _account.Login(model);
            if (result.IsSuccess)
            {
                return RedirectToLocal(returnUrl);
            }
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddUser()
        {
            var model = new AddUserViewModel();            
            return View(await _account.AddUser(model));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddUser(AddUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(await _account.AddUser(model));
            }                
            var result = await _account.Register(model);
            if (result.IsSuccess)
            {
                SetReturnMessage.SuccessMessage("User added successfully. Completion email has been sent to user");
                return RedirectToAction("Index");
            }
            if(result?.Data?.Errors != null)
                AddErrors(result.Data);
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View(await _account.AddUser(model));
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _account.LogOff();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        public IActionResult ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                SetReturnMessage.FailureMessage("Invalid link");
                return RedirectToAction("Index", "Home");
            }
            var model = new ConfirmEmailViewModel
            {
                UserId = userId,
                Code = code
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await _account.ConfirmEmail(model);
            if (result.IsSuccess)
            {
                SetReturnMessage.SuccessMessage("Email confirmation successful. Please login");
                return RedirectToAction("Login");
            }
            if(result?.Data?.Errors != null)
                AddErrors(result.Data);
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }





        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
