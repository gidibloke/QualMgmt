using Application.Core;
using Application.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IUserManagement
    {
        Task<Result<IdentityResult>> Register(AddUserViewModel model);
        Task<AddUserViewModel> AddUser(AddUserViewModel model);
        Task<Result<List<UsersViewModel>>> GetUsersAsync();
        Task<Result<object>> DisableUserAsync(string userId);
        Task<Result<object>> EnableUserAsync(string userId);
        Task<Result<EditUserViewModel>> GetEditViewAsync(string userId);
        Task<Result<object>> ChangeCareHomeAsync(EditUserViewModel model);
        Task<Result<object>> UpdateEmailAsync(EditUserViewModel model);
        //Task<Result<EditUserViewModel>> PopulateDropDown()

    }
}
