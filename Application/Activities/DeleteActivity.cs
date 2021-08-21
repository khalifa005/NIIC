using System;
using System.Threading;
using System.Threading.Tasks;
using Domains;
using MediatR;
using Microsoft.AspNetCore.Http;
using Persistence;

namespace Application.Activities
{
    public class DeleteActivity
    {
        public class Request : IRequest<Response>
        {
            public Guid Id { get; set; }
        }

        public class Response : ApiResponse
        {
            public Activity Activity { get; set; }

            public Response()
            {
            }

            public Response(Activity activity)
            {
                Activity = activity;
                StatusCode = StatusCodes.Status200OK;
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
                //return null obj and right status code in the api response obj
                try
                {
                    var currentActivity = await _context.Activities.FindAsync(request.Id);

                    if (currentActivity == null)
                        return new Response
                        {
                            Activity = null,
                            StatusCode = StatusCodes.Status404NotFound
                        };


                    _context.Remove(currentActivity);

                    var success = await _context.SaveChangesAsync() > 0;

                    if (!success) throw new Exception("can't delete activity entity");

                    return new Response(currentActivity);
                }
                catch (Exception ex)
                {
                    return ApiResponse.Error<Response>("error delete activity", ex);
                }
            }
        }
    }

}