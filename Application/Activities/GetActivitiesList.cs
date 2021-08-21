using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Activities.Dtos;
using Application.Interfaces;
using AutoMapper;
using Domains;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;
using Activity = Domains.Activity;

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
                        .Include(x => x.UserActivities)
                        .ThenInclude(x => x.AppUser)
                        .ThenInclude(x => x.Photos)
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

    public class ActivityFilterUI : FilterUI<ActivityFilter>
    {
        public string City { get; set; }

        public override ActivityFilter GetFilter()
        {
            return new ActivityFilter
            {
                City = City
            };
        }

        public override string SerializeJson()
        {
            throw new NotImplementedException();
        }

        public static ActivityFilterUI Default()
        {
            return new ActivityFilterUI
            {
                City = string.Empty
            };
        }
    }

    public class ActivityFilter : IFilter<Activity>
    {
        public string City { get; set; }
        public IQueryable<Activity> Filter(IQueryable<Activity> query)
        {

            //if (JobStatusId.HasValue)
            //    query = query.Where(x => x.StatusId == JobStatusId);

            if (City is { Length: > 2 })
            {
                City = City.ToLower();

                query = query.Where(x => x.City.ToLower().Contains(City));
            }

            return query;
        }

    }

    public class ActivitySorter : ISort<Activity>
    {
        public OrderOperator? ByCreateDate { get; set; }

        public IOrderedQueryable<Activity> Sort(IQueryable<Activity> query)
        {
            if (ByCreateDate.HasValue)
            {
                // We are sorting by Id because well Id indexes very fast in sorting than date and we know bigger id means created later
                return ByCreateDate switch
                {
                    OrderOperator.Descending => query.OrderByDescending(x => x.Id),
                    _ => query.OrderBy(x => x.Id)
                };
            }

            return query.OrderByDescending(x => x.Id);
        }

        public static ActivitySorter ByCreateDateAsc() => new() { ByCreateDate = OrderOperator.Ascending };

        public static ActivitySorter ByCreateDateDesc() => new() { ByCreateDate = OrderOperator.Descending };
    }

    public class GetActivitiesListPagining
    {
        public class Request : IRequest<Response>
        {
            public readonly ActivityFilter _filter;
            public readonly ISort<Activity> _sorter;
            public readonly int _page;
            public readonly int _pageSize;

            public Request(ActivityFilter filter, ISort<Activity> sorter, int Page, int PageSize)
            {
                _filter = filter;
                _sorter = sorter;
                _page = Page;
                _pageSize = PageSize;
            }
        }

        public class Response : ApiResponse
        {
            public List<ActivityDto> Activities { get; set; }
            public PagingInfo PagingResult { get; set; }
            public Response()
            {
            }

            public Response(List<ActivityDto> activities, PagingInfo pagingResult)
            {
                Activities = activities;
                PagingResult = pagingResult;
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
                 
                    var pagingQuery = new SortedPagingQueryCondition<Activity>(request._filter, request._sorter, request._page, request._pageSize);

                    var queryRelatedData = _context.Activities
                        .Include(x => x.UserActivities)
                        .ThenInclude(x => x.AppUser)
                        .ThenInclude(x => x.Photos)
                        .AsQueryable();

                    var query = QueryBuilder.Paging(queryRelatedData, pagingQuery);
                    //without related data//var query = QueryBuilder.Paging(_context.Activities.AsQueryable(), pagingQuery);

                    var countTotalResult = await query.Count.CountAsync();
                    var activitiesResult = await query.Listing.ToListAsync();
                    
                    var activitiesDtoUsingAutoMapper = _mapper.Map<List<Activity>, List<ActivityDto>>(activitiesResult);

                    var pagingInfo = new PagingInfo
                    {
                        CurrentPage = request._page,
                        PageSize = request._pageSize,
                        TotalResults = countTotalResult
                    };

                    return new Response(activitiesDtoUsingAutoMapper, pagingInfo);
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