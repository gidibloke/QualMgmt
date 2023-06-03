using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class StaffManagementController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
