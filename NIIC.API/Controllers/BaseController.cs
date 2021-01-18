using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace NIIC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        /* The difference is that you don't need to inject IMediator to the constructor of BaseController and to all sub classes of BaseController.
        So it's saves some boilerplate, but also makes the dependency less explicit.
        Side note, Microsoft recommends to prefer the injection over RequestServices*/

        //read about DI in details https://joonasw.net/view/aspnet-core-di-deep-dive
        private IMediator _mediator;
        //Service Locater pattern instead of injecting. 
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

        //we can also do this but then we need to pass IServiceProvider in the inherited controller
        public BaseController(IServiceProvider provider)
        {
            _mediator = provider.GetRequiredService<IMediator>();
        }
        public BaseController()
        {

        }
    }
}
