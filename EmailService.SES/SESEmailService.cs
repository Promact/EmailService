using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using EmailService;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace SESEmailService
{
    public class SESEmailService : IEmailService
    {
        private readonly SESOptions _sesOptions;

        public SESEmailService(SESOptions sesOptions)
        {
            _sesOptions = sesOptions;
        }

        public SESEmailService(IOptions<SESOptions> sesOptions)
        {
            _sesOptions = sesOptions.Value;
        }

        public async Task SendEmailAsync(Mail mail)
        {
            using (var client = new AmazonSimpleEmailServiceClient(_sesOptions.AccessKeyId, _sesOptions.SecretAccessKey, RegionEndpoint.GetBySystemName(_sesOptions.Region)))
            {
                var sendRequest = new SendEmailRequest();

                sendRequest.Source = mail.From;

                sendRequest.Destination = new Destination();

                sendRequest.Destination.ToAddresses = mail.To;

                if (mail.CC != null)
                {
                    sendRequest.Destination.CcAddresses = mail.CC;
                }

                if(mail.BCC != null)
                {
                    sendRequest.Destination.BccAddresses = mail.BCC;
                }

                sendRequest.Message = new Message();
                
                sendRequest.Message.Subject = new Content(mail.Subject);
                
                sendRequest.Message.Body = new Body();

                if (mail.IsBodyHTML)
                {
                    sendRequest.Message.Body.Html = new Content
                    {
                        Charset = "UTF-8",
                        Data = mail.Body
                    };
                }

                else
                {
                    sendRequest.Message.Body.Text = new Content
                    {
                        Charset = "UTF-8",
                        Data = mail.Body
                    };
                }   

                await client.SendEmailAsync(sendRequest);
            }
        }
    }
}
