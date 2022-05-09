using Emailit.Models;
using System.Threading.Tasks;

namespace Emailit.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(EmailMessage message);
    }
}