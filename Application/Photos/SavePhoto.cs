using Application.Interfaces;
using Domains;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Photos
{

    public class SavePhoto
    {
        public class Request : IRequest<Response>
        {
            public IFormFile File { get; set; }

        }

        public class Response : ApiResponse
        {

            public Photo Photo { get; set; }
            public Response(Photo photo)
            {
                Photo = photo;
                StatusCode = StatusCodes.Status200OK;
            }
        }

       

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;
            private readonly IPhotoAccessor _photoAccessor;

            public Handler(DataContext context, IUserAccessor userAccessor, IPhotoAccessor photoAccessor)
            {
                _context = context;
                _userAccessor = userAccessor;
                _photoAccessor = photoAccessor;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {

                var currentLogedUser = await _context.Users.Include(x => x.Photos)
                    .SingleOrDefaultAsync(x => x.UserName == _userAccessor
                    .GetCurrentLogedinUsername());

                //test credential
                var getCurrentLOgedInUserById = _userAccessor.GetCurrentLogedinUserId();

                var uploadPhotoResult = _photoAccessor.AddPhoto(request.File);

                var photoEntity = new Photo
                {
                    Id = uploadPhotoResult.PublicId,
                    Url = uploadPhotoResult.Url
                };

                if (!currentLogedUser.Photos.Any(n => n.IsMain))
                    photoEntity.IsMain = true;

                //user entity tracked by db context 
                currentLogedUser.Photos.Add(photoEntity);

                var success = await _context.SaveChangesAsync() > 0;

                if (!success)
                    throw new RestException(HttpStatusCode.BadRequest,
                        new { Error = "can't save photo" });
                //return Unit.Value;

                return new Response(photoEntity);
            }
        }
    }
}
