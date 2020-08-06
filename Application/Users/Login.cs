using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
            public string Email { get; set; }
            public string Password { get; set; }
        }
        
        public class Response : ApiResponse
        {
            public AppUser User { get; set; }

            public Response()
            {
                
            }

            public Response(AppUser user)
            {
                User = user;
                StatusCode = StatusCodes.Status200OK;
            }
        }

        public class RequestValidator :AbstractValidator<Request>
        {
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

            public Handler(UserManager<AppUser>manager, SignInManager<AppUser>signInManager)
            {
                _manager = manager;
                _signInManager = signInManager;
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
                return new Response(user);
            }
        }

    }
}
