using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Web.Interfaces;

namespace Web.Services
{
    public class SetReturnMessageService : ISetReturnMessages
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly ITempDataDictionaryFactory _factory;

        public SetReturnMessageService(IHttpContextAccessor httpContext, ITempDataDictionaryFactory factory)
        {
            _httpContext = httpContext;
            _factory = factory;
        }

        public void FailureMessage(string message)
        {
            var httpContext = _httpContext.HttpContext;
            var tempData = _factory.GetTempData(httpContext);
            tempData["failure"] = message ?? "Operation failed, please contact admin";
        }

        public void InfoMessage(string message)
        {
            var httpContext = _httpContext.HttpContext;
            var tempData = _factory.GetTempData(httpContext);
            tempData["message"] = message;
        }

        public void SuccessMessage(string message)
        {
            var httpContext = _httpContext.HttpContext;
            var tempData = _factory.GetTempData(httpContext);
            tempData["success"] = message ?? "Operation successful";
        }
    }
}
