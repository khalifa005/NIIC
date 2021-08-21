using Application.Interfaces;
using Application.Validators;
using Domains.Identity;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Users
{
    public class Register
    {

        public class Request :IRequest<Response>
        {
            public string DisplayName { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class Response : ApiResponse
        {
            public UserDto UserDto { get; set; }
            public Response(UserDto userDto)
            {
                UserDto = userDto;
                StatusCode = StatusCodes.Status200OK;
            }
        }

        public class RegisterRequestValidator : AbstractValidator<Request>
        {
            public RegisterRequestValidator()
            {
                RuleFor(x=>x.DisplayName).NotEmpty();
                RuleFor(x=>x.UserName).NotEmpty();
                RuleFor(x=>x.Password).Password();
                RuleFor(x=>x.Email).EmailAddress();
            }
        }



        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly DataContext _context;
            private readonly IJwtGenerator _jwtGenerator;
            private readonly UserManager<AppUser> _userManager;
            
            public Handler(DataContext context, IJwtGenerator jwtGenerator, UserManager<AppUser> userManager)
            {
                _context = context;
                _jwtGenerator = jwtGenerator;
                _userManager = userManager;
            }
            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var isEmailExist = await _context.Users.AnyAsync(x => x.Email == request.Email,cancellationToken);
                if(isEmailExist)
                    throw new RestException(System.Net.HttpStatusCode.BadRequest, new { Email = "already exist" });

                var isUserNameExist = await _context.Users.AnyAsync(x => x.UserName == request.UserName, cancellationToken);
                if (isUserNameExist)
                    throw new RestException(System.Net.HttpStatusCode.BadRequest, new { UserName = "already exist" });

                var userEntity = new AppUser
                {
                    UserName = request.UserName,
                    DisplayName = request.DisplayName,
                    Email = request.Email,
                };

                var result = await _userManager.CreateAsync(userEntity, request.Password);

                if(!result.Succeeded)
                    throw new RestException(HttpStatusCode.BadRequest, new { SaveOperation = "failed" });

                var userDto = new UserDto
                {
                    DisplayName = userEntity.DisplayName,
                    Username = userEntity.UserName,
                    Token = _jwtGenerator.CreateToken(userEntity),
                    Image = userEntity.Photos.FirstOrDefault(x => x.IsMain == true)?.Url
                };
                return new Response(userDto);

            }
        }

    }
}
