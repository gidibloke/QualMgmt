using Application.Interfaces;
using Application.ViewModels;
using Domain.LookupModels;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Web.Extensions;

namespace Web.Controllers
{
    public class StaffQualificationsController : BaseController
    {
        private readonly ISetStaffSession _staffSession;
        private readonly IRepository<Qualification> _qualRepo;
        private readonly IFileUpload _fileUpload;
        private readonly IWebHostEnvironment _env;
        private readonly IRepository<StaffQualification> _staffQualRepository;


        public StaffQualificationsController(ISetStaffSession staffSession, 
            IRepository<StaffQualification> staffQualRepository, 
            IRepository<Qualification> qualRepo, 
            IFileUpload fileUpload, IWebHostEnvironment env)
        {
            _staffSession = staffSession;
            _qualRepo = qualRepo;
            _fileUpload = fileUpload;
            _env = env;
            _staffQualRepository = staffQualRepository;
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

        public async Task<IActionResult> GetQualifications()
        {

            var qualifications = await _staffQualRepository.GetByConditionAndIncludeAsync(condition: x => x.StaffId == StaffId, includes: x => x.Qualification);
            var data = qualifications.Data.Select(x => new
            {
                x.Qualification.Description, x.AwardingOrganisation, x.DateAttainedTo, x.Id
            });
              return Json(new { data }, new JsonSerializerOptions());
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
            if (!ModelState.IsValid)
            {
                await PopulateDropDown(model);
                return View(model);
            }
            var data = Mapper.Map<StaffQualification>(model);
            data.EntBy = User.Identity.GetFullName();
            if(model.FormFile != null)
            {
                data.DocumentPath = await _fileUpload.Upload(model);
            }
            if(string.IsNullOrEmpty(data.DocumentPath))
            {
                ModelState.AddModelError("", "Invalid file. Please upload PDF or remove the uploaded file.");
            }                
            var result = await _staffQualRepository.AddAsync(data);
            if (result.IsSuccess)
            {
                SetReturnMessage.SuccessMessage("Qualification added successfully");
                return RedirectToAction("Index", new {Id = model.StaffId});
            }
            return View();
        }

        public async Task<IActionResult> Edit(int Id)
        {
            var qual = await _staffQualRepository.GetByIdAsync(Id);
            if (qual.IsSuccess)
            {
                var model = Mapper.Map<StaffQualificationViewModel>(qual.Data);
                await PopulateDropDown(model);
                return View(model);  
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(StaffQualificationViewModel model)
        {
            if (!ModelState.IsValid) {
                await PopulateDropDown(model);
                return View(model);
            }
            if (model.FormFile != null && model.DocumentPath != null)
            {
                _fileUpload.DeleteFile(model);
                model.DocumentPath = await _fileUpload.Upload(model);
            }

            var obj = Mapper.Map<StaffQualification>(model);
            if (model.FormFile != null)
            {
                obj.DocumentPath = await _fileUpload.Upload(model);
            }
            var result = await _staffQualRepository.UpdateAsync(obj); 
            if (result.IsSuccess)
            {
                SetReturnMessage.SuccessMessage("Operation successfull");
                return RedirectToAction("Index", new { Id = model.StaffId });
            }
            await PopulateDropDown(model);
            return View(model);
        }

        public async Task<IActionResult> Delete(int Id)
        {
            var model = await _staffQualRepository.GetByIdAsync(Id);
            if (model.IsSuccess == false)
                return Json(new { success = false, message = "Cannot find entry" }, new JsonSerializerOptions());
            if(model.Data.DocumentPath != null)
            {
                var res = Mapper.Map<StaffQualificationViewModel>(model.Data);
                res.DownloadFileLink = Path.GetFileName(res.DocumentPath);
                _fileUpload.DeleteFile(res);
            }
            var result = await _staffQualRepository.DeleteAsync(model.Data);
            if (result.IsSuccess)
                return Json(new { success = true, message = "Qualification removed" });
            return Json(new { success = false, message = "Operation failed, please try again later" });
        }


        public IActionResult Download(string fileName)
        {

            var fileLocation = Path.Combine(_env.WebRootPath, "uploads", StaffId.ToString(), fileName);
            if(System.IO.File.Exists(fileLocation))
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(fileLocation);
                return File(fileBytes, "application/octet-stream");
            }
            return Json(new { success = false, message=" File not found" });
        }

        public async Task PopulateDropDown(StaffQualificationViewModel model)
        {
            var qualifications = await _qualRepo.GetAllAsync();
            model.Qualifications = qualifications.Data.Select(x => new SelectListItem
            {
                Text = x.Description,
                Value = x.Id.ToString()
            });
            if(model.DocumentPath != null)
            {
                model.DownloadFileLink = Path.GetFileName(model.DocumentPath);
            }
        }
    }
}
