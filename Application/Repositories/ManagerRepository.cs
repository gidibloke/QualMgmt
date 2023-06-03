using Application.Core;
using Application.Interfaces;
using Application.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System.Security.Claims;

namespace Application.Repositories
{
    internal class ManagerRepository : IManagerDashboard
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContext;

        public ManagerRepository(ApplicationDbContext context, IHttpContextAccessor httpContext)
        {
            _context = context;
            _httpContext = httpContext;
        }
        public async Task<Result<StaffViewModel>> GetAllStaff()
        {
            var ctx = _httpContext.HttpContext;
            var query = _context.Staff.AsQueryable();
            if (ctx.User.IsInRole("Admin"))
            {
                var staff = await query.Select(x => new StaffViewModel
                {

                }).ToListAsync();
                
            }
            else
            {
                var homeId = int.TryParse(ctx.User.FindFirst("CareHomeId").Value, out int carehomeId);
                if (homeId)
                {
                    query = query.Where(x => x.CareHomeId == carehomeId);
                    var staff = await query.Select(x => new StaffViewModel
                    {

                    }).ToListAsync();
                }
            }
        }

        private async Task<List<StaffViewModel>> GetManagerAsync()
        {
            var ctx = _httpContext.HttpContext;
            var query = _context.Staff.AsQueryable();
        }
        
        private async Task<List<StaffViewModel>> GetAdminAsync()
        {
            var ctx = _httpContext.HttpContext;
            var query = _context.Staff.AsQueryable();
        }
    }
}
