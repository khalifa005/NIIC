using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Activities.Dtos;
using AutoMapper;
using Domains;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class GetActivity
    {
        public class Request : IRequest<Response>
        {
            public Guid Id { get; set; }
        }

        public class Response : ApiResponse
        {
            public ActivityDto Activity { get; set; }

            public Response()
            {
            }

            public Response(ActivityDto activity)
            {
                Activity = activity;
                StatusCode = StatusCodes.Status200OK;
            }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                try
                {
                    var activity =
                        await _context.Activities
                        .Include(x => x.UserActivities)
                        .ThenInclude(x => x.AppUser)
                        .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                    //pass exception to api response generic error method internal error
                    //or use res exception that we bvuild
                    if (activity == null)
                        throw new Exception("activity is null");


                    var activityDtotoUsingAutoMapper = _mapper.Map<Activity, ActivityDto>(activity);
                    //or
                    var activityDtoWithoutAutoMapper = new ActivityDto()
                    {
                        Title = activity.Title,
                        UserActivities = activity.UserActivities.Select(x =>
                        {
                            return new AttendeeDto()
                            {
                                DisplayName = x.AppUser.DisplayName,
                                Username = x.AppUser.UserName
                            };
                        }).ToList()
                    };

                    return new Response(activityDtotoUsingAutoMapper);
                }
                catch (Exception ex)
                {
                    return ApiResponse.Error<Response>("get activity", ex);
                }
            }
        }
    }

}