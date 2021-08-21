using Application.Interfaces;
using Application.UserProfile;
using Application.UserProfile.Dto;
using Domains;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Followings
{

    public class GetFollowings
    {
        public class Request : IRequest<Response>
        {
            public string Username { get; set; }
            public string Predicate { get; set; }

        }

        public class Response : ApiResponse
        {

            public List<ProfileDto> Profiles { get; set; }
            public Response(List<ProfileDto> profiles)
            {
                Profiles = profiles;
                StatusCode = StatusCodes.Status200OK;
            }
        }

        public class RequestValidator : AbstractValidator<Request>
        {
            public RequestValidator()
            {

            }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly DataContext _context;
            private readonly IProfileReader _profileReader;

            public Handler(DataContext context, IProfileReader profileReader)
            {
                _context = context;
                _profileReader = profileReader;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {

                //here we didn't make any database access
                var query = _context.Followings.AsQueryable();

                var userFollowings = new List<UserFollowing>();

                var profiles = new List<ProfileDto>();

                //var result = request.Predicate switch
                //{
                //    "followers" => await query.Where(x => x.Target.UserName == request.Username)
                //    .ToListAsync(cancellationToken),

                //    _ => await query.Where(x => x.Observer.UserName == request.Username)
                //   .ToListAsync(cancellationToken),
                //};


                switch (request.Predicate)
                {
                    case "followers":
                        {
                            userFollowings = await query.Where(x => x.Target.UserName == request.Username).Include(x => x.Observer).ToListAsync(cancellationToken);

                            foreach (var item in userFollowings)
                            {
                                profiles.Add(await _profileReader.ReadProfile(item.Observer.UserName)); 
                            }
                            break;
                        }

                    case "following":
                        {
                            userFollowings = await query.Where(x => x.Observer.UserName == request.Username).Include(x => x.Target).ToListAsync(cancellationToken);

                            foreach (var item in userFollowings)
                            {
                                profiles.Add(await _profileReader.ReadProfile(item.Target.UserName));
                            }
                            break;
                        }
                }

                return new Response(profiles);
            }
        }
    }
}
