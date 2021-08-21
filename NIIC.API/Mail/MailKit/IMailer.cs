using System.Threading.Tasks;

namespace NIIC.API.Mail.MailKit
{
    public interface IMailer
    {
        Task SendEmailAsync(string email, string subject, string body);
    }
}
