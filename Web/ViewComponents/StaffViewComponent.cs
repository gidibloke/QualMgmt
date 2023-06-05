using Application.Interfaces;
using Application.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Web.Interfaces;

namespace Web.ViewComponents
{
    public class StaffViewComponent : ViewComponent
    {
        private readonly ISetReturnMessages _setReturn;

        public StaffViewComponent(ISetReturnMessages setReturn)
        {
            _setReturn = setReturn;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var data = HttpContext.Session.GetString("StaffVariables");
            var result = JsonSerializer.Deserialize<StaffSessionViewModel>(data);
            if(result == null)
            {
                _setReturn.FailureMessage("Problems getting staff details. Please try again");
                HttpContext.Response.Redirect("/StaffManagement/Index");
            }
            return View(result);
        }
    }
}
