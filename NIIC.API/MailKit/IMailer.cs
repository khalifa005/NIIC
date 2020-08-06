using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NIIC.API.MailKit
{
    public interface IMailer
    {
        Task SendEmailAsync(string email, string subject, string body);
    }
}
