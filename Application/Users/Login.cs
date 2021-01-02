using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Domains.Identity;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Application.Users
{
    public class Login
    {
        public class Request : IRequest<Response>
        {
            //should create login input
            public string Email { get; set; }
            public string Password { get; set; }
        }
        
        public class Response : ApiResponse
        {
            public UserDto User { get; set; }

            public Response()
            {
                
            }

            public Response(UserDto user)
            {
                User = user;
                StatusCode = StatusCodes.Status200OK;
            }
        }

        public class RequestValidator :AbstractValidator<Request>
        {
            // AbstractValidator should take from login input 
            public RequestValidator()
            {
                RuleFor(x => x.Email).EmailAddress().NotEmpty();
                RuleFor(x => x.Password).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly UserManager<AppUser> _manager;
            private readonly SignInManager<AppUser> _signInManager;
            private readonly IJwtGenerator _jwtGenerator;

            public Handler(UserManager<AppUser>manager, SignInManager<AppUser>signInManager, IJwtGenerator jwtGenerator)
            {
                _manager = manager;
                _signInManager = signInManager;
                _jwtGenerator = jwtGenerator;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var user = await _manager.FindByEmailAsync(request.Email);

                if (user == null)
                {
                    throw new RestException(HttpStatusCode.Unauthorized);
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

                if (!result.Succeeded)
                {
                    throw new RestException(HttpStatusCode.Unauthorized);
                }

                //generate token
                return new Response(new UserDto()
                {
                    DisplayName = user.DisplayName,
                    Image = null,
                    Username = user.UserName,
                    Token = _jwtGenerator.CreateToken(user)
                });
            }
        }

    }
}
