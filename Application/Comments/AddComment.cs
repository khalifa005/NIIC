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
using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch.Adapters;
using Application.Comments.Dto;

namespace Application.Comments
{

    public class AddComment
    {
        public class Request : IRequest<Response>
        {
            public Guid ActivityId { get; set; }
            public int? CommentId { get; set; }
            public string Username { get; set; }
            public string CommentContent { get; set; }

        }

        public class Response : ApiResponse
        {
            public CommentDto Comment{ get; set; }
            public Response(CommentDto comment)
            {
                Comment = comment;
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

                var activity = await _context.Activities
                    .SingleOrDefaultAsync(x => x.Id == request.ActivityId);

                if(activity is null)
                    throw new RestException(HttpStatusCode.BadRequest,
                          new { Error = " activity entity is null" });

                var user = await _context.Users
                    .SingleOrDefaultAsync(x => x.UserName == request.Username);

                if (user is null)
                    throw new RestException(HttpStatusCode.BadRequest,
                          new { Error = " user entity is null" });

                var comment = new Comment()
                {
                    Activity = activity,
                    Author = user, 
                    Content = request.CommentContent,
                    CreatedAt = DateTime.Now,
                };

                //we can get comment parent and pass it 
                if(request.CommentId.HasValue)
                    comment.ParentCommentId = request.CommentId;

                activity.Comments.Add(comment);

                var success = await _context.SaveChangesAsync() > 0;

                if (!success)
                    throw new RestException(HttpStatusCode.BadRequest,
                        new { Error = "can't save activity entity" });

                var commentDto = _mapper.Map<CommentDto>(comment);

                return new Response(commentDto);
            }
        }
    }
}
