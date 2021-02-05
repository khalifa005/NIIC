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
    public class CurrentUser
    {
        public class Request : IRequest<Response>
        {
           
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


        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly UserManager<AppUser> _manager;
            private readonly IJwtGenerator _jwtGenerator;
            private readonly IUserAccessor _userAccessor;

            public Handler(UserManager<AppUser> manager, IJwtGenerator jwtGenerator, IUserAccessor userAccessor )
            {
                _manager = manager;
                _jwtGenerator = jwtGenerator;
                _userAccessor = userAccessor;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var user = await _manager.FindByNameAsync(_userAccessor.GetCurrentLogedinUsername());

                if (user == null)
                {
                    throw new RestException(HttpStatusCode.NotFound, new { User = "user in not exist"});
                }

                //mapping user entity to user dto
                var mapedUser = new UserDto()
                {
                    DisplayName = user.DisplayName,
                    Image = user.Photos.FirstOrDefault(x=> x.IsMain == true)?.Url,
                    Username = user.UserName,
                    Token = _jwtGenerator.CreateToken(user)//generate token
                };

                return new Response(mapedUser);
            }
        }
    }
}
