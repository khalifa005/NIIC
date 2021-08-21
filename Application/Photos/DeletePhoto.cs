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

    public class DeletePhoto
    {
        public class Request : IRequest<Response>
        {
            public string PublicId { get; set; }

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
            private readonly IPhotoAccessor _photoAccessor;

            public Handler(DataContext context, IUserAccessor userAccessor, IPhotoAccessor photoAccessor)
            {
                _context = context;
                _userAccessor = userAccessor;
                _photoAccessor = photoAccessor;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {

                var currentLogedUser = await _context.Users.Include(x=> x.Photos)
                    .SingleOrDefaultAsync(x => x.UserName == _userAccessor.GetCurrentLogedinUsername());


                var photo = currentLogedUser.Photos.FirstOrDefault(x=> x.Id == request.PublicId);

                if(photo == null)
                    throw new RestException(HttpStatusCode.BadRequest, new { photo = "not found" });

                if (photo.IsMain)
                    throw new RestException(HttpStatusCode.BadRequest, new { photo = "can't delete main photo" });

                var result = _photoAccessor.DeletePhoto(request.PublicId);
                
                if (result == null)
                    throw new Exception("can't delete photo");

                currentLogedUser.Photos.Remove(photo);

                var success = await _context.SaveChangesAsync() > 0;

                if (!success)
                    throw new RestException(HttpStatusCode.BadRequest,
                        new { Error = "can't save activity entity" });
                //return Unit.Value;

                return new Response();
            }
        }
    }
}
