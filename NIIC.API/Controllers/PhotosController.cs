using Application.Photos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NIIC.API.Controllers
{
    public class PhotosController : BaseController
    {

        //api controller will match value sent from postman (form-data--> key = File)
        //then in the SavePhoto.Request we should have similar value name give hint to make it look inside [fromForm]
        [HttpPost]
        public async Task<SavePhoto.Response> Add([FromForm]SavePhoto.Request request)
        {
            //return await Mediator.Send(new SavePhoto.Request{file = request.file });
            return await Mediator.Send(request);
        }

        [HttpDelete("{id}")]
        public async Task<DeletePhoto.Response> Add(string id)
        {
            return await Mediator.Send(new DeletePhoto.Request{ PublicId = id});
        }
        
        [HttpPost("{id}/setmain")]
        public async Task<SaveSetPhotoAsMain.Response> SetPhotoAsMain(string id)
        {
            return await Mediator.Send(new SaveSetPhotoAsMain.Request{ PublicId = id});
        }

    }
}
