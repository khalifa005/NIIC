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
   
    public class DeleteAttend
    {
        public class Request : IRequest<Response>
        {
            public Guid Id { get; set; }

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
                  .SingleOrDefaultAsync(x => x.Id == request.Id);

                if (activity == null)
                    throw new RestException(HttpStatusCode.NotFound,
                         new { Error = "can't find activity" });


                var currentLogedUser = await _context.Users
                    .SingleOrDefaultAsync(x => x.UserName == _userAccessor.GetCurrentLogedinUsername());

                var userActivity = await _context.UserActivities
                    .FirstOrDefaultAsync(x=> x.AppUserId == currentLogedUser.Id && x.ActivityId == activity.Id);

                if(userActivity == null)
                    throw new RestException(HttpStatusCode.BadRequest,
                        new { Obj ="user activity is not exist" });
                
                if(userActivity.IsHost)
                    throw new RestException(HttpStatusCode.BadRequest,
                        new { Obj ="host cannot remove him self" });

                _context.UserActivities.Remove(userActivity);

                var success = await _context.SaveChangesAsync() > 0;

                if (!success)
                    throw new RestException(HttpStatusCode.BadRequest,
                        new { Error = "can't save delete activity entity" });
                //return Unit.Value;

                return new Response();
            }
        }
    }
}
