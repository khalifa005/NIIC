using Application.Comments;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NIIC.API.SignalR
{
    public class ChatHub  : Hub
    {
        private readonly IMediator _mediator;

        public ChatHub(IMediator mediator)
        {
            _mediator = mediator;
        }

        //teh method name SendComment important because we will use taht on the client to invoke this particular method
        public async Task SendComment(AddComment.Request request)
        {
            //we need to make the token available in hub context ??
            //we couldn't use user access-or because it relay on http context 
            var username = Context.User?.Claims?.FirstOrDefault(x=> x.Type == ClaimTypes.NameIdentifier)?.Value;

            request.Username = username;

            var comment = await _mediator.Send(request);

            //we now need to send (show ) this comment in real time to all clients subscribed to this hub

            await Clients.All.SendAsync("ReceiveComment", comment);
        }
    }
}
