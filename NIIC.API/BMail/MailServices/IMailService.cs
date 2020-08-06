using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domains.BMail;

namespace NIIC.API.BMail.MailServices
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
        Task SendWelcomeEmailAsync(WelcomeRequest request);
    }
}
