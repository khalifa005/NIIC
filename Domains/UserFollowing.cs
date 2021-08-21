using Domains.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domains
{
    public class UserFollowing
    {
        public string ObserverId { get; set; }
        public AppUser Observer { get; set; }

        public string TargetId { get; set; }
        public AppUser Target { get; set; }

    }
}
