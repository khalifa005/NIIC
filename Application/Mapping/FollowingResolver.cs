using Application.Activities.Dtos;
using Application.Interfaces;
using AutoMapper;
using Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application.Mapping
{
    class FollowingResolver : IValueResolver<UserActivity, AttendeeDto, bool>//bool is return type for resolver
    {
        private readonly DataContext _context;
        private readonly IUserAccessor _accessor;

        public FollowingResolver(DataContext context , IUserAccessor accessor)
        {
            _context = context;
            _accessor = accessor;
        }
        public bool Resolve(UserActivity source, AttendeeDto destination, bool destMember, ResolutionContext context)
        {
            var currentLogedUser = _context.Users.Include(u=> u.Followings).SingleOrDefaultAsync(x=> x.UserName == _accessor.GetCurrentLogedinUsername()).Result;

            if(currentLogedUser.Followings.Any(x=> x.TargetId == source.AppUserId))
                return true;

            return false;
        }
    }
}
