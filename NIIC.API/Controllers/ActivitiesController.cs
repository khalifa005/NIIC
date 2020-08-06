using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Application.Activities;
using MediatR;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;


namespace NIIC.API.Controllers
{
    public class ActivitiesController : BaseController
    {
       
        [HttpGet]
        public async Task<GetActivitiesList.Response> Get(CancellationToken cancellation)
        {
            return await Mediator.Send(new GetActivitiesList.Request(), cancellation);
        }

       

        [HttpGet("{id}")]
        public async Task<GetActivity.Response> Get(Guid id)
        {
            return await Mediator.Send(new GetActivity.Request{Id = id });
        }

        [HttpPost]
        public async Task<CreateActivity.Response> Post(CreateActivity.Request activity)
        {
            return await Mediator.Send(activity);
        }

        
        [HttpPut("{id}")]
        public async Task<EditActivity.Response> Put(Guid id, EditActivity.Request activity)
        {
            activity.Id = id;
            return await Mediator.Send(activity);
        }

        [HttpDelete("{id}")]
        public async Task<DeleteActivity.Response> Delete(Guid id)
        {
            return await Mediator.Send(new DeleteActivity.Request { Id = id });
        }
    }
}
