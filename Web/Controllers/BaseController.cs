using Application.Core;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Web.Interfaces;

namespace Web.Controllers
{
    public class BaseController : Controller
    {
        private ISetReturnMessages _returnMessage;
        protected ISetReturnMessages SetReturnMessage => _returnMessage ??= HttpContext.RequestServices.GetRequiredService<ISetReturnMessages>();

        private IMapper _mapper;
        protected IMapper Mapper => _mapper ??= HttpContext.RequestServices.GetRequiredService<IMapper>();
    }

}
