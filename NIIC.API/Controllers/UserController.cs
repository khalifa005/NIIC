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
    public class UserController : BaseController
    {
        [AllowAnonymous]
        [HttpPost("login")]

        public async Task<Login.Response> Login(Login.Request request)
        {
            return await Mediator.Send(request);
        }

        //using return response makes it easy to change to any data type in the future improve scalability
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<Register.Response> Register(Register.Request request)
        {

            //return await Mediator.Send(new Register.Request{ input = RegisterInput })
            return await Mediator.Send(request);
        }

        [HttpGet]
        public async Task<CurrentUser.Response> GetUser()
        {
            return await Mediator.Send(new CurrentUser.Request());
        }

    }

}
