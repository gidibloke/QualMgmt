using Application.Core;
using Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ISetStaffSession
    {
        Task<Result<StaffSessionViewModel>> SetStaffSession(string Id);
    }
}
