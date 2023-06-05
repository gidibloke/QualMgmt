using Application.Core;
using Application.Interfaces;
using Application.Repositories;
using Application.ViewModels;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class SetSessionService : ISetStaffSession
    {
        private readonly IRepository<Staff> _repository;

        public SetSessionService(IRepository<Staff> repository)
        {

            _repository = repository;
        }

        public async Task<Result<StaffSessionViewModel>> SetStaffSession(string Id)
        {
            var staffSession = new StaffSessionViewModel();
            var staff = await _repository.GetByIdAsync(Id);
            if (staff.IsSuccess)
            {
                staffSession.FullName = $"{staff.Data.FirstName} {staff.Data.LastName}";
                staffSession.DateOfBirth = staff.Data.DateOfBirth.Value.ToString("dd-MM-yyyy");
                staffSession.JobTitle = staff.Data.JobTitle;
                return Result<StaffSessionViewModel>.Success(staffSession);
            }
            return Result<StaffSessionViewModel>.Failure("Error fetching staff details. Please try again");
        }
    }
}
