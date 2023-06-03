using Application.Core;
using Application.Interfaces;
using Application.ViewModels;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public class AdminDashboardRepository : IAdminDashboard
    {
        private readonly ApplicationDbContext _context;

        public AdminDashboardRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result<AdminDashboardViewModel>> GetDashboardAsync()
        {
            var users = await _context.Users.Select(x => new {x.Id, x.IsEnabled}).ToListAsync();
            var careHomes = await _context.CareHomes.CountAsync();
            var data = new AdminDashboardViewModel
            {
                Users = users.Count,
                CareHomes = careHomes,
                EnabledUsers = users.Where(x => x.IsEnabled).Count()
            };
            return Result<AdminDashboardViewModel>.Success(data);            
        }
    }
}
