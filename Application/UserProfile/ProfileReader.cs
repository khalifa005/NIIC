using Application.Interfaces;
using Application.UserProfile.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UserProfile
{
    public interface IProfileReader
    {
        Task<ProfileDto> ReadProfile(string username);
    }
    public class ProfileReader : IProfileReader
    {
        private readonly IUserAccessor _accessor;
        private readonly DataContext _context;

        public ProfileReader(IUserAccessor accessor, DataContext context)
        {
            _accessor = accessor;
            _context = context;
        }
        public async Task<ProfileDto> ReadProfile(string username)
        {
            var user = await _context.Users.Include(x => x.Followers)
                .Include(x => x.Followings)
                .Include(x => x.Photos)
                .SingleOrDefaultAsync(x => x.UserName == username);

            if (user is null)
            {
                throw new RestException(System.Net.HttpStatusCode.NotFound
                    , new { user = "user with this username is not found" });
            }

            var currentLogedInUser = await _context.Users
                .SingleOrDefaultAsync(x => x.Id == _accessor.GetCurrentLogedinUserId());

            var profile = new ProfileDto
            {
                DisplayName = user.DisplayName,
                Bio = user.Bio,
                MainImage = user.Photos.FirstOrDefault(x => x.IsMain == true)?.Url,
                Username = user.UserName,
                Images = user.Photos,
                FollowersCount = user.Followers.Count,
                FollowingsCount = user.Followings.Count
            };

            if (user.Followers.Any(x => x.ObserverId == currentLogedInUser.Id))
                profile.IsFollowing = true;

            return profile;

        }
    }
}
