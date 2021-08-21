using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application;
using Domains.BMail;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NIIC.API.Mail.BMail.MailServices;

namespace NIIC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly IMailService _mailService;
        public MailController(IMailService mailService)
        {
             _mailService = mailService;
        }
        [HttpPost("send")]
        public async Task<IActionResult> SendMail([FromForm] MailRequest request)
        {
            try
            {
                await _mailService.SendEmailAsync(request);
                return Ok();
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        [HttpPost("welcome")]
        public async Task<IActionResult> SendWelcomeMail([FromForm] WelcomeRequest request)
        {
            try
            {
                await _mailService.SendWelcomeEmailAsync(request);
                return Ok();
            }
            catch (Exception ex)
            {

                throw new RestException(System.Net.HttpStatusCode.BadRequest,ex);
            }

        }
    }
}
