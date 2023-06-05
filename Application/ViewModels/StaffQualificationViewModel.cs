using Domain.LookupModels;
using Domain.Models;
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
        public DateTime? DateAttainedFrom { get; set; }

        [Display(Name = "To")]
        public DateTime? DateAttainedTo { get; set; }

        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Display(Name = "Grade")]
        public string Grade { get; set; }
        public bool RenewableYN { get; set; }

        [Display(Name = "Expiry date")]
        public DateTime? ExpiryDate { get; set; }
        
        [Display(Name = "Any evidence (pdf only)")]
        public string DocumentPath { get; set; }
        public string EntBy { get; set; }
        public IFormFile FormFile { get; set; }
    }
}
