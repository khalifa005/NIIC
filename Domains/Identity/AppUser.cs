using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace Domains.Identity
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public string Bio { get; set; }

        public List<UserActivity> UserActivities { get; set; }
        public List<Photo> Photos { get; set; }
        public List<UserFollowing> Followings { get; set; }
        public List<UserFollowing> Followers { get; set; }

        public AppUser()
        {
            UserActivities = new List<UserActivity>();
            Photos = new List<Photo>();
            Followers = new List<UserFollowing>();
            Followings = new List<UserFollowing>();
        }

    }
}
