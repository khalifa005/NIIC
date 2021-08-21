using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Domains;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Persistence;

namespace Application.Activities
{
    public class EditActivity
    {
        public class Request : IRequest<Response>
        {
            public Guid? Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string Category { get; set; }
            public DateTime Date { get; set; }
            public string City { get; set; }
            public string Venue { get; set; }
        }

        public class RequestValidator : AbstractValidator<Request>
        {
            public RequestValidator()
            {
                RuleFor(x => x.Title).NotEmpty();
                RuleFor(x => x.Description).NotEmpty();
                RuleFor(x => x.Category).NotEmpty();
                RuleFor(x => x.Date).NotEmpty();
                RuleFor(x => x.City).NotEmpty();
                RuleFor(x => x.Venue).NotEmpty();
            }
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
                var currentActivity = await _context.Activities.FindAsync(request.Id);
                
                //return not found null obj and right status code in the api response obj
                if (currentActivity == null)
                    throw new RestException(HttpStatusCode.NotFound, new {currentActivity = "Not Found"});
                
                currentActivity.Title = request.Title ?? currentActivity.Title;
                currentActivity.Description = request.Description ?? currentActivity.Description;
                currentActivity.Category = request.Category ?? currentActivity.Category;
                currentActivity.Date = request.Date;
                currentActivity.City = request.City ?? currentActivity.City;
                currentActivity.Venue = request.Venue ?? currentActivity.Venue;


                var success = await _context.SaveChangesAsync() > 0;

                if (!success) throw new Exception("can't edit activity entity");

                return new Response(currentActivity);
            }
        }
    }

}