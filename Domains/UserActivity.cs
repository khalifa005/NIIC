using Domains.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domains
{
    public class UserActivity
    {
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        public Guid ActivityId { get; set; }
        public Activity Activity { get; set; }

        public DateTime DateJoined { get; set; }
        public bool IsHost { get; set; }
    }
}
