using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Domains;
using FluentValidation;
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
                    for (var i = 0; i < 10; i++)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await Task.Delay(2000, cancellationToken);
                        _log.LogInformation($"task {i} has completed");
                    }

                    var activities = await _context.Activities.ToListAsync(cancellationToken);

                    return new Response(activities);
                }
                catch (Exception ex) when (ex is TaskCanceledException)
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
                    var activity =
                        await _context.Activities.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
                    //pass exception to api response generic error method internal error
                    if (activity == null)
                        throw new Exception("activity is null");

                    return new Response(activity);
                }
                catch (Exception ex)
                {
                    return ApiResponse.Error<Response>("get activity", ex);
                }
            }
        }
    }

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

            public Handler(DataContext context)
            {
                _context = context;
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