using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string  LastName { get; set; }

        //Naming by convention. If naming is different, properties can be set using linq in OnModelCreating or using data attributes
        public int? CareHomeId { get; set; }
        public virtual CareHome CareHome { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateRegistered { get; set; }
    }
}
