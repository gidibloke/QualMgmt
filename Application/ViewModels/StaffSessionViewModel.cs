using Application.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels
{
    public class StaffSessionViewModel
    {
        public string FullName { get; set; }
        public string JobTitle { get; set; }
        public string DateOfBirth { get; set; }

    }
}
