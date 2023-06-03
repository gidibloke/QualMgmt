using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels
{
    public class UsersViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Fullname { get; set; }
        public string CareHomeName { get; set; }
        public string RoleName { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool IsEnabled { get; set; }
    }
}
