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
            private readonly IProfileReader _profileReader;

            public Handler(IProfileReader profileReader)
            {
                _profileReader = profileReader;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                return new Response(await _profileReader.ReadProfile(request.Username));
            }
        }
    }
}
