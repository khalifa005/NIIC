﻿using Application.UserProfile;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NIIC.API.Controllers
{
    public class ProfilesController : BaseController
    {

        [HttpGet("{username}")]
        public async Task<GetProfile.Response> GetProfile(string username)
        { 
            return await Mediator.Send(new GetProfile.Request{Username = username});
        }
    }
}
