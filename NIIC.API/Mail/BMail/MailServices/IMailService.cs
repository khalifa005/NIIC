using System.Threading.Tasks;
using Domains.BMail;

namespace NIIC.API.Mail.BMail.MailServices
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
        Task SendWelcomeEmailAsync(WelcomeRequest request);
    }
}
