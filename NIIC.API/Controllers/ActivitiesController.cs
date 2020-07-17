using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Activities;
using Domains;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace NIIC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]  //returtn 404 ...
    public class ActivitiesController //: ControllerBase //return ok status //Controller return or generate view
    {
        private readonly IMediator _mediator;

        public ActivitiesController(IMediator mediator)
        {
            _mediator = mediator;
        }
        // GET: api/<ActivitiesController>
        [HttpGet]
        public async Task<GetActivitiesList.Response> Get(CancellationToken cancellation)
        {
            return await _mediator.Send(new GetActivitiesList.Request(), cancellation);
        }

        //public async Task<ActionResult<GetActivitiesList.Response>> Get()
        //{
        //    return Ok(await _mediator.Send(new GetActivitiesList.Request()));
        //}


        // GET api/<ActivitiesController>/5
        [HttpGet("{id}")]
        public async Task<GetActivity.Response> Get(Guid id)
        {
            return await _mediator.Send(new GetActivity.Request{Id = id });
        }

        // POST api/<ActivitiesController>
        [HttpPost]
        public async Task<CreateActivity.Response> Post(CreateActivity.Request activity)
        {
            return await _mediator.Send(activity);
        }

        //[HttpPost]
        //public async Task<SaveActivity.Response> Post(SaveActivity.Request activity)
        //{
        //    return await _mediator.Send(activity);
        //}

        // PUT api/<ActivitiesController>/5
        [HttpPut("{id}")]
        public async Task<EditActivity.Response> Put(Guid id, EditActivity.Request activity)
        {
            activity.Id = id;
            return await _mediator.Send(activity);
        }

        // DELETE api/<ActivitiesController>/5
        [HttpDelete("{id}")]
        public async Task<DeleteActivity.Response> Delete(Guid id)
        {
            return await _mediator.Send(new DeleteActivity.Request {Id = id});
        }
    }
}
