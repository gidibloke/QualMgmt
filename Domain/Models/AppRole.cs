using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class AppRole : IdentityRole
    {
        public AppRole() : base()
        {
            
        }
        public AppRole(string readableName) : base(readableName)
        {
            ReadableName = readableName;
        }
        public virtual string ReadableName { get; set; }
    }
}
