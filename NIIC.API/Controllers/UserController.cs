using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NIIC.API.Controllers
{
    [AllowAnonymous]
    public class UserController : BaseController
    {
        [HttpPost("login")]

        public async Task<Login.Response> Login(Login.Request request)
        {
            return await Mediator.Send(request);
        }

        //using return response makes it easy to change to any data type in the future improve scalability
       

    }
}
