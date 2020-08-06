using System;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer
{
    public class ApplicationUser : IdentityUser
    {
        public string DisplayName { get; set; }
    }
}
