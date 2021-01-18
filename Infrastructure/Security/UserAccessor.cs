using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Security
{
    public class UserAccessor : IUserAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public string GetCurrentLogedinUsername()
        {
            //User? to make it optional sometimes we wouldn't have user
            var username = _httpContextAccessor.HttpContext.User?.Claims
                .FirstOrDefault(x=> x.Type == ClaimTypes.NameIdentifier)?.Value;

            return username;
        }
    }
}
