using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace Domains.Identity
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }

        public List<UserActivity> UserActivities { get; set; }

        public AppUser()
        {
            UserActivities = new List<UserActivity>();
        }

    }
}
