using Application.Core;
using Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAdminDashboard
    {
        Task<Result<AdminDashboardViewModel>> GetDashboardAsync();
    }
}
