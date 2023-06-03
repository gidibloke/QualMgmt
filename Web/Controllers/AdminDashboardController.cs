using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class AdminDashboardController : BaseController
    {
        private readonly IAdminDashboard _adminDashboard;

        public AdminDashboardController(IAdminDashboard adminDashboard)
        {
            _adminDashboard = adminDashboard;
        }
        public async Task<IActionResult> Index()
        {
            var result = await _adminDashboard.GetDashboardAsync();
            return View(result.Data);
        }
    }
}
