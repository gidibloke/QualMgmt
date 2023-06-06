using Application.Core;
using Application.Interfaces;
using Application.ViewModels;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;
using Web.Extensions;

namespace Web.Controllers
{
    public class StaffManagementController : BaseController
    {
        private readonly IRepository<Staff> _staffRepo;
        private readonly IRepository<CareHome> _carehomeRepo;
        private readonly IFileUpload _fileUpload;

        public StaffManagementController(IRepository<Staff> staffRepo, IRepository<CareHome> carehomeRepo, IFileUpload fileUpload)
        {
            _staffRepo = staffRepo;
            _carehomeRepo = carehomeRepo;
            _fileUpload = fileUpload;
        }
        public IActionResult Index()
        {
            HttpContext.Session.Clear();
            return View();
        }

        public async Task<IActionResult> GetAllStaff()
        {
            if (User.IsInRole("Manager"))
            {
                var careHome = User.Identity.GetCareHomeId();
                var result = await _staffRepo.GetByConditionAndIncludeAsync(condition: x => x.CareHomeId == Convert.ToInt32(careHome));
                var data = result.Data.Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    x.JobTitle,
                    x.DateCreated,
                    x.Id
                });
                return Json(new { data }, new JsonSerializerOptions());
            }
            else
            {
                var result = await _staffRepo.GetAllAsync();
                var data = result.Data.Select(x => new
                {
                    x.FirstName, x.LastName, x.JobTitle, x.DateCreated, x.Id
                });
                return Json(new { data }, new JsonSerializerOptions());
            }

        }


        public async Task<IActionResult> Create()
        {
            var model = new StaffViewModel();
            await PopulateDropdown(model);
            return View(model);


        }


        [HttpPost]
        public async Task<IActionResult> Create(StaffViewModel model)
        {
            if(!ModelState.IsValid)
            {
                await PopulateDropdown(model);
                return View(model);
            }
            var data = Mapper.Map<Staff>(model);
            data.DateCreated = DateTime.Now;
            data.EntBy = User.Identity.GetFullName();
            var result = await _staffRepo.AddAsync(data);
            if(result.IsSuccess == false)
            {
                SetReturnMessage.FailureMessage("Operation failed");
                await PopulateDropdown(model);
                return View(model);
            }
            SetReturnMessage.SuccessMessage("Operation successful");
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(string Id)
        {
            if(Id == null)
            {
                SetReturnMessage.FailureMessage("Operation failed. Please try again");
                return RedirectToAction("Index");
            }
            var model = await _staffRepo.GetByIdAsync(Id);
            var result = Mapper.Map<StaffViewModel>(model.Data);
            await PopulateDropdown(result);
            return View(result);

        }

        [HttpPost]
        public async Task<IActionResult> Edit(StaffViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdown(model);
                return View(model);
            }
            var data = Mapper.Map<Staff>(model);
            var result = await _staffRepo.UpdateAsync(data);
            if (result.IsSuccess == false)
            {
                SetReturnMessage.FailureMessage("Operation failed");
                await PopulateDropdown(model);
                return View(model);
            }
            SetReturnMessage.SuccessMessage("Operation successful");
            return RedirectToAction("Index");
        }



        public async Task<IActionResult> Delete(string Id)
        {
            var model = await _staffRepo.GetByIdAsync(Id);
            if (model.IsSuccess == false)
                return Json(new { success = false, message = "Cannot find entry" }, new JsonSerializerOptions());            
            var result = await _staffRepo.DeleteAsync(model.Data);
            if (result.IsSuccess)
            {
                _fileUpload.DeleteAll(Id);
                return Json(new { success = true, message = "Staff removed" });

            }
            return Json(new { success = false, message = "Operation failed, please try again later" });
        }

        private async Task PopulateDropdown(StaffViewModel model)
        {
            if (User.IsInRole("Manager"))
            {
                model.CareHomeId = Convert.ToInt32(User.Identity.GetCareHomeId());
            }
            else
            {
                var careHomes = await _carehomeRepo.GetAllAsync();
                model.CareHomes = careHomes.Data.Select(x => new SelectListItem{
                    Text = x.HomeName,
                    Value = x.Id.ToString()

                }).ToList();
            }

        }
    }
}
