using Application.Core;
using Application.ViewModels;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAccount
    {
        Task<Result<SignInResult>> Login(LoginViewModel model);
        Task<Result<IdentityResult>> Register(AddUserViewModel model);
        Task<AddUserViewModel> AddUser(AddUserViewModel model);

        Task LogOff();
        Task<Result<IdentityResult>> ConfirmEmail(ConfirmEmailViewModel model);

    }
}
