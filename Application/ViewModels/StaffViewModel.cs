using Application.Attributes;
using Domain.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels
{
    public class StaffViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "First name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]

        public string LastName { get; set; }
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        [MinimumAge(16, ErrorMessage = "Minimum age of staffs is 16")]

        public DateTime? DateOfBirth { get; set; }
        [Display(Name = "Job Title")]

        public string JobTitle { get; set; }
        [Display(Name = "Annual Salary")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Only number is allowed")]
        public string AnnualSalary { get; set; }
        public DateTime? DateCreated { get; set; }

        [Display(Name = "Choose care home")]
        public int? CareHomeId { get; set; }
        public IEnumerable<SelectListItem> CareHomes { get; set; }
        public string EntBy { get; set; }

    }

    public class StaffViewModelValidator : AbstractValidator<StaffViewModel>
    {
        public StaffViewModelValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name is required");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name is required");
            RuleFor(x => x.DateOfBirth).Cascade(CascadeMode.Stop).NotEmpty().WithMessage("Date of birth is required").LessThan(x => DateTime.Now).WithMessage("Date of birth cannot be in the future");
            RuleFor(x => x.JobTitle).NotEmpty().WithMessage("Job title is required");
            RuleFor(x => x.AnnualSalary).NotEmpty().WithMessage("Annual salary is required");

        }
    }
}
