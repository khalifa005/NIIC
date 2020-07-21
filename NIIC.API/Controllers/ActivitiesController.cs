using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Activities;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace NIIC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]  
    public class ActivitiesController : ControllerBase 
    {
        private readonly IMediator _mediator;

        public ActivitiesController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<GetActivitiesList.Response> Get(CancellationToken cancellation)
        {
            return await _mediator.Send(new GetActivitiesList.Request(), cancellation);
        }

       

        [HttpGet("{id}")]
        public async Task<GetActivity.Response> Get(Guid id)
        {
            return await _mediator.Send(new GetActivity.Request{Id = id });
        }

        [HttpPost]
        public async Task<CreateActivity.Response> Post(CreateActivity.Request activity)
        {
            return await _mediator.Send(activity);
        }

        
        [HttpPut("{id}")]
        public async Task<EditActivity.Response> Put(Guid id, EditActivity.Request activity)
        {
            activity.Id = id;
            return await _mediator.Send(activity);
        }

        [HttpDelete("{id}")]
        public async Task<DeleteActivity.Response> Delete(Guid id)
        {
            return await _mediator.Send(new DeleteActivity.Request { Id = id });
        }
    }
}
