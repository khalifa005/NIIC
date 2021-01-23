using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Domains;
using FluentValidation;
using MediatR;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.Activities
{
    public class CreateActivity
    {
        public class Request : IRequest<Response>
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string Category { get; set; }
            public DateTime Date { get; set; }
            public string City { get; set; }
            public string Venue { get; set; }
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

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _context = context;
                _userAccessor = userAccessor;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                try
                {
                    var activity = new Activity();

                    activity = new Activity
                    {
                        Id = request.Id,
                        Title = request.Title,
                        Description = request.Description,
                        Category = request.Category,
                        Date = request.Date,
                        City = request.City,
                        Venue = request.Venue
                    };

                    _context.Activities.Add(activity);

                    var currentLogedUser = await _context.Users
                        .SingleOrDefaultAsync(x=> x.UserName == _userAccessor.GetCurrentLogedinUsername());

                    //attendee
                    var userActivity = new UserActivity()
                    {
                        AppUser = currentLogedUser,
                        Activity = activity,
                        IsHost = true,
                        DateJoined = DateTime.Now
                    };

                    _context.UserActivities.Add(userActivity);

                    var success = await _context.SaveChangesAsync() > 0;

                    if (!success) throw new Exception("can't save activity entity");
                    //return Unit.Value;

                    return new Response(activity);
                }
                catch (Exception ex)
                {
                    return ApiResponse.Error<Response>("error save activity", ex);
                }
            }
        }
    }

}