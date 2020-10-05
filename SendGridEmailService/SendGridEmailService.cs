using EmailService;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Linq;
using System.Threading.Tasks;

namespace SendGridEmailService
{
    public class SendGridEmailService : IEmailService
    {
        private readonly SendGridOptions _sendGridOptions;

        public SendGridEmailService(SendGridOptions sendGridOptions)
        {
            _sendGridOptions = sendGridOptions;
        }

        public SendGridEmailService(IOptions<SendGridOptions> sendGridOptions)
        {
            _sendGridOptions = sendGridOptions.Value;
        }

        public async Task SendEmailAsync(Mail mail)
        {
            var sendGridClient = new SendGridClient(_sendGridOptions.APIKey);

            var sendGridMessage = new SendGridMessage();

            sendGridMessage.From = new EmailAddress(mail.From);

            sendGridMessage.AddTos(mail.To.Select(x => new EmailAddress(x)).ToList());

            if (mail.CC != null)
            {
                sendGridMessage.AddCcs(mail.CC.Select(x => new EmailAddress(x)).ToList());
            }

            if (mail.BCC != null)
            {
                sendGridMessage.AddBccs(mail.BCC.Select(x => new EmailAddress(x)).ToList());
            }

            sendGridMessage.Subject = mail.Subject;

            if (mail.IsBodyHTML)
            {
                sendGridMessage.HtmlContent = mail.Body;
            }

            else
            {
                sendGridMessage.PlainTextContent = mail.Body;
            }

            await sendGridClient.SendEmailAsync(sendGridMessage);
        }
    }
}
