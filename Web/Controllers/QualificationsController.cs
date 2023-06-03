using Application.Interfaces;
using Application.ViewModels;
using AutoMapper;
using Domain.LookupModels;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Web.Interfaces;

namespace Web.Controllers
{
    public class QualificationsController : BaseController
    {
        private readonly IRepository<Qualification> _repository;

        public QualificationsController(IRepository<Qualification> repository)
        {
            _repository = repository;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetQualifications()
        {
            var data = await _repository.GetAllAsync();
            return Json(new { data = data.Data }, new JsonSerializerOptions());
        }

        public IActionResult Create()
        {
            var model = new QualificationViewModel();
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Create(QualificationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var data = Mapper.Map<Qualification>(model);
            data.DateCreated = DateTime.Now;
            var result = await _repository.AddAsync(data);
            if (result.IsSuccess)
            {
                SetReturnMessage.SuccessMessage("Operation successful");
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int Id)
        {
            var model = await _repository.GetByIdAsync(Id);
            if (model.IsSuccess == false)
            {
                return RedirectToAction(nameof(Index));
            }
            var data = Mapper.Map<QualificationViewModel>(model.Data);
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(QualificationViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var data = Mapper.Map<Qualification>(model);
            var result = await _repository.UpdateAsync(data);
            if (result.IsSuccess)
            {
                SetReturnMessage.SuccessMessage("Operation successful");
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int Id)
        {
            var model = await _repository.GetByIdAsync(Id);
            if (model.IsSuccess == false)
                return Json(new { success = false, message = "Cannot find entry" }, new JsonSerializerOptions());
            var result = await _repository.DeleteAsync(model.Data);
            if (result.IsSuccess)
                return Json(new { success = true, message = "Care home removed" });
            return Json(new { success = false, message = "Operation failed, please try again later" });
        }

    }
}
