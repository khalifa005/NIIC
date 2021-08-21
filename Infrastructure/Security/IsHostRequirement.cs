using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Security
{
    public class IsHostRequirement : IAuthorizationRequirement
    {
    }

    public class IsHostRequirementHandler : AuthorizationHandler<IsHostRequirement>
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _accessor;

        public IsHostRequirementHandler(DataContext context, IHttpContextAccessor accessor)
        {
            _context = context;
            _accessor = accessor;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsHostRequirement requirement)
        {
            var username = _accessor.HttpContext.User?.Claims
             .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            var activityId = _accessor.HttpContext.GetRouteData().Values["id"].ToString();

            if (string.IsNullOrWhiteSpace(activityId))
                context.Fail();

            var activity = _context.Activities
                .Include(x => x.UserActivities)
                .ThenInclude(x => x.AppUser)
                .FirstOrDefault(x => x.Id.ToString() == activityId);

            if (activity is null)
                context.Fail();

            if (activity.UserActivities.Any(x => x.IsHost == true && x.AppUser.UserName == username))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
