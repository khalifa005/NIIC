using Domains;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Application.UserProfile.Dto
{
    public class ProfileDto
    {
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Bio { get; set; }
        public string MainImage { get; set; }

        [JsonPropertyName("Following")]
        public bool IsFollowing { get; set; }

        public int FollowingsCount { get; set; }
        public int FollowersCount { get; set; }

        public List<Photo> Images{ get; set; }
    }
}
