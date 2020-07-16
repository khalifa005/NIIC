using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Domains;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Activities
{
    public class GetActivitiesList
    {
        public class Request : IRequest<Response>
        {
            
        }

        public class Response : ApiResponse
        {
            public List<Activity> Activities { get; set; }
            public Response()
            {
                
            }

            public Response(List<Activity> activities)
            {
                Activities = activities;
                StatusCode = StatusCodes.Status200OK;
            }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly DataContext _context;
            private readonly ILogger<GetActivitiesList> _log;

            public Handler(DataContext context, ILogger<GetActivitiesList> log)
            {
                _context = context;
                _log = log;
            }
            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                try
                {
                    for (int i = 0; i < 10; i++)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await Task.Delay(2000, cancellationToken);
                        _log.LogInformation($"task {i} has completed");
                    }
                    var activities = await _context.Activities.ToListAsync(cancellationToken);

                    return new Response(activities);
                }
                catch (Exception ex) when(ex is TaskCanceledException)
                {
                    _log.LogInformation("task was cancelled");
                    return ApiResponse.Error<Response>("get activities", ex);
                }
            }
        }
    }

    public class GetActivity
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
                try
                {
                    var activity = await _context.Activities.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

                    return new Response(activity);
                }
                catch (Exception ex)
                {
                    return ApiResponse.Error<Response>("get activity", ex);
                }
            }
        }
    }

    public class SaveActivity
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

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                try
                {
                    var activity = new Activity
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

                    var success = await _context.SaveChangesAsync() > 0;

                    if (!success)
                    {
                        throw new Exception("can't save activity entity");
                    }
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
