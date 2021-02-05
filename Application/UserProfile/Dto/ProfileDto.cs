using Domains;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.UserProfile.Dto
{
    public class ProfileDto
    {
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Bio { get; set; }
        public string MainImage { get; set; }
        public List<Photo> Images{ get; set; }
    }
}
