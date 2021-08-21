using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Activities.Dtos;
using Application.Interfaces;
using AutoMapper;
using Domains;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Followings
{

    public class SaveFollowing
    {
        public class Request : IRequest<Response>
        {
            public string TargetUsername { get; set; }

        }

        public class Response : ApiResponse
        {


            public Response()
            {
                StatusCode = StatusCodes.Status200OK;
            }
        }

        public class RequestValidator : AbstractValidator<Request>
        {
            public RequestValidator()
            {
                RuleFor(x=>x.TargetUsername).NotEmpty();
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

                var currentLogedUser = await _context.Users
                    .SingleOrDefaultAsync(x => x.UserName == _userAccessor.GetCurrentLogedinUsername());

                var targetUser = await _context.Users
                    .SingleOrDefaultAsync(x => x.UserName == request.TargetUsername);

                if (targetUser is null)
                    throw new RestException(HttpStatusCode.NotFound,
                        new { Error = "user is not found" });


                var following = await _context.Followings
                    .SingleOrDefaultAsync(x => x.ObserverId == currentLogedUser.Id && x.TargetId == targetUser.Id);

                if (following is not null)
                    throw new RestException(HttpStatusCode.BadRequest,
                        new { Error = "following is already exist" });

                var newFollowing = new UserFollowing
                {
                    TargetId = targetUser.Id,
                    ObserverId = currentLogedUser.Id,
                };

                _context.Followings.Add(newFollowing);

                var success = await _context.SaveChangesAsync() > 0;

                if (!success)
                    throw new RestException(HttpStatusCode.BadRequest,
                        new { Error = "can't save entity" });

                return new Response();
            }
        }
    }
}
