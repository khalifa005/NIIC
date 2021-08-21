using Application.Followings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NIIC.API.Controllers
{
    [Route("api/profiles")]

    public class FollowersController : BaseController
    {
        [HttpPost("{username}/follow")]
        public async Task<SaveFollowing.Response> Follow(string username)
        {
            return await Mediator.Send(new SaveFollowing.Request{TargetUsername = username });
        }

        [HttpDelete("{username}/follow")]
        public async Task<DeleteFollowing.Response> UnFollow(string username)
        {
            return await Mediator.Send(new DeleteFollowing.Request { TargetUsername = username });
        }


        [HttpGet("{username}/follow")]
        public async Task<GetFollowings.Response> GetFollowings(string username, string predicate)
        {
            return await Mediator.Send(new GetFollowings.Request
            { Username= username , Predicate  =predicate});
        }
    }
}
