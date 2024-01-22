using System.Threading.Tasks;

namespace EmailService
{
    public interface IEmailService
    {
        Task SendEmailAsync(Mail mail);

        Task SendTemplatedEmailAsync(TemplatedEmailRequest templatedEmailRequest);
    }
}
