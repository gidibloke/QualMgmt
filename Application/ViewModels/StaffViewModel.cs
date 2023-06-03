using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels
{
    public class StaffViewModel
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string JobTitle { get; set; }
        public string AnnualSalary { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? CareHomeId { get; set; }
    }
}
