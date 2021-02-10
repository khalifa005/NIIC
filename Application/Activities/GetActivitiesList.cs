using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Application.Activities.Dtos;
using AutoMapper;
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
            public List<ActivityDto> Activities { get; set; }

            public Response()
            {
            }

            public Response(List<ActivityDto> activities)
            {
                Activities = activities;
                StatusCode = StatusCodes.Status200OK;
            }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly DataContext _context;
            private readonly ILogger<GetActivitiesList> _log;
            private readonly IMapper _mapper;

            public Handler(DataContext context, ILogger<GetActivitiesList> log, IMapper mapper)
            {
                _context = context;
                _log = log;
                _mapper = mapper;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                try
                {
                    //cancellation token test with delaying request test on console
                    //for (var i = 0; i < 10; i++)
                    //{
                    //    cancellationToken.ThrowIfCancellationRequested();
                    //    await Task.Delay(2000, cancellationToken);
                    //    _log.LogInformation($"task {i} has completed");
                    //}

                    var activities = await _context.Activities
                        .Include(x=> x.UserActivities)
                        .ThenInclude(x=> x.AppUser)
                        .ThenInclude(x=> x.Photos)
                        .ToListAsync(cancellationToken);

                    //if we want to lazy load related data ,
                    //install ef proxy package and configure it in startup
                    //add virtual key word before all related data in domain entities 
                    var activitiesDtoUsingAutoMapper = _mapper.Map<List<Activity>, List<ActivityDto>>(activities);
                    return new Response(activitiesDtoUsingAutoMapper);
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _log.LogInformation("task was canceled");
                    return ApiResponse.Error<Response>("get activities", ex);
                }
            }
        }
    }

}