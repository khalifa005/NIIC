using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Activities.Dtos;
using Application.UserProfile.Dto;
using AutoMapper;
using Domains;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.UserProfile
{
    public class GetProfile
    {
        public class Request : IRequest<Response>
        {
            public string Username { get; set; }

        }

        public class Response : ApiResponse
        {
            public ProfileDto Profile { get; set; }

            public Response(ProfileDto profile)
            {
                Profile = profile;
                StatusCode = StatusCodes.Status200OK;
            }
        }

        public class RequestValidator : AbstractValidator<Request>
        {
            public RequestValidator()
            {
                RuleFor(x=> x.Username).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {

                var user = await _context.Users.Include(x=> x.Photos)
                    .SingleOrDefaultAsync(x => x.UserName == request.Username);

                if(user is null)
                    new RestException(HttpStatusCode.NoContent,
                        new { Error = "entity is null" });

                var profile = new ProfileDto
                {
                    DisplayName = user.DisplayName,
                    Bio = user.Bio,
                    MainImage = user.Photos.FirstOrDefault(x=> x.IsMain == true)?.Url,
                    Username = user.UserName,
                    Images = user.Photos
                };

                return new Response(profile);
            }
        }
    }
}
