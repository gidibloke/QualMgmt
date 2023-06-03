using Application.Interfaces;
using Application.ViewModels;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CareHomeManagementController : BaseController
    {
        private readonly IRepository<CareHome> _carehHomeRepo;

        public CareHomeManagementController(IRepository<CareHome> carehHomeRepo)
        {
            _carehHomeRepo = carehHomeRepo;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetCareHomes()
        {
            var data = await _carehHomeRepo.GetAllAsync();
            return Json(new { data = data.Data }, new JsonSerializerOptions());
        }

        public IActionResult Create()
        {
            var model = new CareHomeViewModel();
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Create(CareHomeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var data = Mapper.Map<CareHome>(model);
            data.DateCreated = DateTime.Now;
            var result = await _carehHomeRepo.AddAsync(data);
            if (result.IsSuccess)
            {
                SetReturnMessage.SuccessMessage("Operation successful");
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int Id)
        {
            var model = await _carehHomeRepo.GetByIdAsync(Id);
            if (model.IsSuccess == false)
            {
                return RedirectToAction(nameof(Index));
            }
            var data = Mapper.Map<CareHomeViewModel>(model.Data);
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CareHomeViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var data = Mapper.Map<CareHome>(model);
            var result = await _carehHomeRepo.UpdateAsync(data);
            if (result.IsSuccess)
            {
                SetReturnMessage.SuccessMessage("Operation successful");
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int Id)
        {
            var model = await _carehHomeRepo.GetByIdAsync(Id);
            if (model.IsSuccess == false)
                return Json(new { success = false, message = "Cannot find entry" }, new JsonSerializerOptions());
            var result = await _carehHomeRepo.DeleteAsync(model.Data);
            if (result.IsSuccess)
                return Json(new { success = true, message = "Care home removed" });
            return Json(new { success = false, message = "Operation failed, please try again later" });
        }


    }
}
