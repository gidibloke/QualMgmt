using Application.Interfaces;
using Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;
using System.Text.Json;

namespace Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserManagementController : BaseController
    {
        private readonly IUserManagement _userManagement;

        public UserManagementController(IUserManagement userManagement)
        {
            _userManagement = userManagement;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetUsers()
        {
            var result =  await _userManagement.GetUsersAsync();
            if (result.IsSuccess == false)
                return Json(new { sucess = false }, new JsonSerializerOptions());
            return Json(new { data = result.Data }, new JsonSerializerOptions());

        }

        public async Task<IActionResult> AddUser()
        {
            var model = new AddUserViewModel();
            return View(await _userManagement.AddUser(model));
        }


        [HttpPost]
        public async Task<IActionResult> AddUser(AddUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(await _userManagement.AddUser(model));
            }
            var result = await _userManagement.Register(model);
            if (result.IsSuccess)
            {
                SetReturnMessage.SuccessMessage("User added successfully. Completion email has been sent to user");
                return RedirectToAction("Index");
            }
            AddErrors(result.Data);
            return View(await _userManagement.AddUser(model));
        }

        [HttpPost]
        public async Task<IActionResult> DisableUser(string Id)
        {
            var result = await _userManagement.DisableUserAsync(Id);
            if (result.IsSuccess == false)
                return Json(new { success = false, message = result.ErrorMessage }, new JsonSerializerOptions());
            return Json(new { success = true, message = "User disabled successfully" }, new JsonSerializerOptions());
        }

        [HttpPost]
        public async Task<IActionResult> EnableUser(string Id)
        {
            var result = await _userManagement.EnableUserAsync(Id);
            if (result.IsSuccess == false)
                return Json(new { success = false, message = result.ErrorMessage }, new JsonSerializerOptions());
            return Json(new { success = true, message = "User enabled successfully" }, new JsonSerializerOptions());
        }

        public async Task<IActionResult> Edit(string Id)
        {
            var result = await _userManagement.GetEditViewAsync(Id);
            if(result.IsSuccess == false)
            {
                SetReturnMessage.FailureMessage("Error loading user");
                return View(nameof(Index));
            }
            return View(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeHome(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Problem changing care home" }, new JsonSerializerOptions());
            }
            var result = await _userManagement.ChangeCareHomeAsync(model);
            if(result.IsSuccess == false)
                return Json(new { success = false, message = "Problem changing care home" }, new JsonSerializerOptions());
            return Json(new { success = true, message = "Care home successfully changed" }, new JsonSerializerOptions());
        }

        [HttpPost]
        public async Task<IActionResult> ChangeEmail(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Problem updating email" }, new JsonSerializerOptions());
            }
            var result = await _userManagement.UpdateEmailAsync(model);
            if(result.IsSuccess == false)
                return Json(new { success = false, message = "Problem updating email" }, new JsonSerializerOptions());
            return Json(new { success = true, message = "Email successfully update. Confirmation email has been sent to user" }, new JsonSerializerOptions());
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
