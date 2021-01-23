using Application.Interfaces;
using Domains;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Activities
{
    public class CreateAttend
    {
        public class Request : IRequest<Response>
        {
            public Guid ActivityId { get; set; }

        }

        public class Response : ApiResponse
        {


            public Response()
            {
                StatusCode = StatusCodes.Status200OK;
            }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _context = context;
                _userAccessor = userAccessor;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {

                var activity = await _context.Activities
                    .SingleOrDefaultAsync(x => x.Id == request.ActivityId);

                if(activity == null) 
                    throw new RestException(HttpStatusCode.NotFound,
                         new { Error = "can't find activity" });

                var currentLogedUser = await _context.Users
                    .SingleOrDefaultAsync(x => x.UserName == _userAccessor.GetCurrentLogedinUsername());

                var userActivity = await _context.UserActivities
                    .SingleOrDefaultAsync(x => x.ActivityId == request.ActivityId && x.AppUserId == currentLogedUser.Id);

                if (userActivity != null)
                    throw new RestException(HttpStatusCode.BadRequest,
                         new { Error = "attend is already exist" });

                var attend = new UserActivity()
                {
                    Activity = activity,
                    AppUser = currentLogedUser,
                    DateJoined = DateTime.Now,
                    IsHost = false
                };

                _context.UserActivities.Add(attend);

                var success = await _context.SaveChangesAsync() > 0;

                if (!success)
                    throw new RestException(HttpStatusCode.BadRequest,
                        new { Error = "can't save activity entity" });
                //return Unit.Value;

                return new Response();
            }
        }
    }


   
}
