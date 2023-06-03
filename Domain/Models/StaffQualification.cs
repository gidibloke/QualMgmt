using Domain.LookupModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    //Use nullable type becuase if there is a bug and the default value for non nullable type is entered. There will be a problem with data integrity
    public class StaffQualification
    {
        public int Id { get; set; }
        public int? QualificationId { get; set; }
        public virtual Qualification Qualification { get; set; }
        public int? StaffId { get; set; }
        public virtual Staff Staff { get; set; }
        public string AwardingOrganisation { get; set; }
        public DateTime? DateAttainedFrom { get; set; }
        public DateTime? DateAttainedTo { get; set; }
        public string Subject { get; set; }
        public string Grade { get; set; }
        public bool? RenewableYN { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string DocumentPath { get; set; }
        public string EntBy { get; set; }



    }
}
