using System;
using System.Collections.Generic;
using System.Linq;
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
using Microsoft.Extensions.Options;
using NIIC.Application.ApplicationSettings;

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
            // AbstractValidator should check on request -> login input 
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
            private readonly IOptions<Jwt> _jwt;

            public Handler(UserManager<AppUser>manager, SignInManager<AppUser>signInManager, IJwtGenerator jwtGenerator, IOptions<Jwt> jwt)
            {
                _manager = manager;
                _signInManager = signInManager;
                _jwtGenerator = jwtGenerator;
                _jwt = jwt;
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
                    //custom exception middelware 
                    throw new RestException(HttpStatusCode.Unauthorized);
                }

                //mapping user entity to user dto
                var mapedUser = new UserDto()
                {
                    DisplayName = user.DisplayName,
                    Image = user.Photos.FirstOrDefault(x => x.IsMain == true)?.Url,
                    Username = user.UserName,
                    Token = _jwtGenerator.CreateToken(user)//generate token
                };

                return new Response(mapedUser);
            }
        }

    }

    //or we can do 

    public class LoginInput
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public LoginInput()
        {

        }

        public class LoginInputValidator : AbstractValidator<LoginInput>
        {
            // AbstractValidator should check on request -> login input 
            public LoginInputValidator()
            {
                RuleFor(x => x.Email).EmailAddress().NotEmpty();
                RuleFor(x => x.Password).NotEmpty();
            }
        }
    }

    //public class GetUser
    //{
    //    public class Request : IRequest<Response>
    //    {
    //        //should create login input
    //        public LoginInput LoginInput { get; set; }
    //    }

    //    public class Response : ApiResponse
    //    {
    //        public UserDto User { get; set; }

    //        public Response()
    //        {

    //        }

    //        public Response(UserDto user)
    //        {
    //            User = user;
    //            StatusCode = StatusCodes.Status200OK;
    //        }
    //    }

    //    public class Handler : IRequestHandler<Request, Response>
    //    {
    //        private readonly UserManager<AppUser> _manager;
    //        private readonly SignInManager<AppUser> _signInManager;
    //        private readonly IJwtGenerator _jwtGenerator;

    //        public Handler(UserManager<AppUser> manager, SignInManager<AppUser> signInManager, IJwtGenerator jwtGenerator)
    //        {
    //            _manager = manager;
    //            _signInManager = signInManager;
    //            _jwtGenerator = jwtGenerator;
    //        }

    //        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    //        {
    //            var user = await _manager.FindByEmailAsync(request.LoginInput.Email);

    //            if (user == null)
    //            {
    //                throw new RestException(HttpStatusCode.Unauthorized);
    //            }

    //            var result = await _signInManager.CheckPasswordSignInAsync(user, request.LoginInput.Password, false);

    //            if (!result.Succeeded)
    //            {
    //                throw new RestException(HttpStatusCode.Unauthorized);
    //            }

    //            //generate token
    //            return new Response(new UserDto()
    //            {
    //                DisplayName = user.DisplayName,
    //                Image = null,
    //                Username = user.UserName,
    //                Token = _jwtGenerator.CreateToken(user)
    //            });
    //        }
    //    }
    //}
}
