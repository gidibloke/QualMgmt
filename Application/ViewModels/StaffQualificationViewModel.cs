using Application.Attributes;
using Domain.LookupModels;
using Domain.Models;
using FluentValidation;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels
{
    public class StaffQualificationViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Type")]
        public int? QualificationId { get; set; }
        public IEnumerable<SelectListItem> Qualifications { get; set; }
        public Guid? StaffId { get; set; }

        [Display(Name = "Awarding Institution")]
        public string AwardingOrganisation { get; set; }

        [Display(Name = "From")]
        [DataType(DataType.Date)]
        [NoFutureDates]
        public DateTime? DateAttainedFrom { get; set; }

        [Display(Name = "To")]
        [DataType(DataType.Date)]

        public DateTime? DateAttainedTo { get; set; }

        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Display(Name = "Grade")]
        public string Grade { get; set; }
        public bool RenewableYN { get; set; }

        [Display(Name = "Expiry date")]
        [DataType(DataType.Date)]

        public DateTime? ExpiryDate { get; set; }
        
        [Display(Name = "Any evidence (pdf only)")]
        public string DocumentPath { get; set; }
        public string EntBy { get; set; }
        public IFormFile FormFile { get; set; }
        public string DownloadFileLink { get; set; }

    }

    public class StaffQualificationValidation : AbstractValidator<StaffQualificationViewModel>
    {
        public StaffQualificationValidation()
        {
            RuleFor(x => x.QualificationId).NotEmpty().WithMessage("Please selecy qualification type");
            RuleFor(x => x.AwardingOrganisation).NotEmpty().WithMessage("Please enter name of awarding body/institution");
            RuleFor(x => x.DateAttainedFrom).NotEmpty().WithMessage("Please enter date of enrollment");
            RuleFor(x => x.DateAttainedTo).NotEmpty().WithMessage("Please enter end date");
            RuleFor(x => x.Subject).NotEmpty().WithMessage("Please enter subject");
            When(x => x.RenewableYN == true,() => {
                RuleFor(x => x.ExpiryDate).NotEmpty().WithMessage("If qualification is renewable, please enter expiry date");
            });
            RuleFor(x => x.DateAttainedFrom).LessThanOrEqualTo(x => x.DateAttainedTo).WithMessage("'From' date cannot be after To date");

        }
    }
}
