using Application.Interfaces;
using Application.ViewModels;
using Domain.LookupModels;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;
using Web.Extensions;

namespace Web.Controllers
{
    public class StaffQualificationsController : BaseController
    {
        private readonly ISetStaffSession _staffSession;
        private readonly IRepository<Qualification> _qualRepo;
        private readonly IRepository<StaffQualification> _staffRepository;


        public StaffQualificationsController(ISetStaffSession staffSession, IRepository<StaffQualification> staffRepository, IRepository<Qualification> qualRepo)
        {
            _staffSession = staffSession;
            _qualRepo = qualRepo;
            _staffRepository = staffRepository;
        }
        public async Task<IActionResult> Index(string Id)
        {
            if(Id == null)
            {
                SetReturnMessage.InfoMessage("Please select a staff member");
                return RedirectToAction("Index", "StaffManagement");
            }
            HttpContext.Session.SetString("StaffId", Id);
            var sessionVariable = await _staffSession.SetStaffSession(Id);
            HttpContext.Session.SetString("StaffVariables", JsonSerializer.Serialize(sessionVariable.Data));
            return View();
        }

        public async Task<IActionResult> Create()
        {
            if (StaffId == null)
                return RedirectToAction("Index", "StaffManagement");
            var qualification = new StaffQualificationViewModel
            {
                StaffId = StaffId,
                EntBy = User.Identity.GetFullName()
            };
            await PopulateDropDown(qualification);
            return View(qualification);
        }

        [HttpPost]
        public async Task<IActionResult> Create(StaffQualificationViewModel model)
        {
            return View();
        }

        public async Task PopulateDropDown(StaffQualificationViewModel model)
        {
            var qualifications = await _qualRepo.GetAllAsync();
            model.Qualifications = qualifications.Data.Select(x => new SelectListItem
            {
                Text = x.Description,
                Value = x.Id.ToString()
            });
        }
    }
}
