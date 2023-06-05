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

        public StaffManagementController(IRepository<Staff> staffRepo, IRepository<CareHome> carehomeRepo)
        {
            _staffRepo = staffRepo;
            _carehomeRepo = carehomeRepo;
        }
        public IActionResult Index()
        {
            HttpContext.Session.Clear();
            return View();
        }

        public async Task<IActionResult> GetAllStaff()
        {
            var result = await _staffRepo.GetAllAsync();
            var data = result.Data.Select(x => new
            {
                x.FirstName, x.LastName, x.JobTitle, x.DateCreated, x.Id
            });
            return Json(new { data }, new JsonSerializerOptions());
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
