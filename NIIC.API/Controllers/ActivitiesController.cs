using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Application.Activities;
using Application.ApplicationSettings;
using Domains;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using SD.LLBLGen.Pro.LinqSupportClasses.ExpressionClasses;

namespace NIIC.API.Controllers
{
    public class ActivitiesController : BaseController
    {

        [HttpGet]
        public async Task<GetActivitiesListPagining.Response> Get(string city ,CancellationToken cancellation)
        {
            //move to separate function

            var filterUI = ActivityFilterUI.Default();
            //filterUI.City = "london";
            filterUI.City = city;
            var filter = filterUI.GetFilter();

            var sorter = ActivitySorter.ByCreateDateDesc();


            return await Mediator.Send(new GetActivitiesListPagining.Request(filter, sorter, Page: 1, PageSize: 5), cancellation);
        }

        ////without sorting and filtering
        //[HttpGet]
        //public async Task<GetActivitiesList.Response> Get(CancellationToken cancellation)
        //{
        //    return await Mediator.Send(new GetActivitiesList.Request(), cancellation);
        //}

        ////or 
        //[HttpGet]
        //public async Task<ActionResult<GetActivitiesList.Response>> GetList(CancellationToken cancellation)
        //{
        //    return await Mediator.Send(new GetActivitiesList.Request(), cancellation);
        //}


        [HttpGet("{id}")]
        [Authorize] //only loged in user can see this if not return 401 unauthorized
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
        //[Authorize (Roles = "admin")] //only admin user can see this
        [Authorize (Policy = Policies.IsActivityHost)]
        //only loged in with IsActivityHostUser
        //policy can see this if not return 403 forbidden
        public async Task<EditActivity.Response> Put(Guid id, EditActivity.Request activity)
        {
            activity.Id = id;
            return await Mediator.Send(activity);
        }


        [HttpDelete("{id}")]
        [Authorize(Policy = Policies.IsActivityHost)]
        public async Task<DeleteActivity.Response> Delete(Guid id)
        {
            return await Mediator.Send(new DeleteActivity.Request { Id = id });
        }

        [HttpPost("{id}/attend")]
        public async Task <CreateAttend.Response> Attend(Guid id)
        {
            //{id}/attend is convention based so params need to be the same as it Guid id 
            return await Mediator.Send(new CreateAttend.Request{ ActivityId = id});
        }

        [HttpDelete("{id}/attend")]
        public async Task<DeleteAttend.Response> DeleteAttend(Guid id)
        {
            //{id}/attend is convention based so params need to be the same as it Guid id 
            return await Mediator.Send(new DeleteAttend.Request { Id = id });
        }
    }
}
