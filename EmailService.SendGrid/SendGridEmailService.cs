using EmailService;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Errors.Model;
using SendGrid.Helpers.Mail;
using System;
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
            try
            {
                var sendGridClient = new SendGridClient(_sendGridOptions.APIKey);

                var sendGridMessage = new SendGridMessage
                {
                    From = new SendGrid.Helpers.Mail.EmailAddress(mail.From.Email, mail.From.Name),
                    Subject = mail.Subject,
                    PlainTextContent = mail.Body, // Assuming you have a plain text body
                    HtmlContent = mail.Body // Assuming you have an HTML body
                };

                sendGridMessage.AddTos(mail.To.Select(x => new SendGrid.Helpers.Mail.EmailAddress(x.Email, x.Name)).ToList());

                // Additional options can be set here, like attachments, headers, etc.
                if (mail.Attachments != null && mail.Attachments.Any())
                {
                    foreach (var item in mail.Attachments)
                    {
                        sendGridMessage.AddAttachment(item.FileName, Convert.ToBase64String(item.Content), item.ContentType);
                    }
                }

                await sendGridClient.SendEmailAsync(sendGridMessage);
            }
            catch (SendGridInternalException ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

        public async Task SendTemplatedEmailAsync(TemplatedEmailRequest templatedEmailRequest)
        {
            try
            {
                var sendGridClient = new SendGridClient(_sendGridOptions.APIKey);
                var message = new SendGridMessage
                {
                    From = new SendGrid.Helpers.Mail.EmailAddress(templatedEmailRequest.From.Email, templatedEmailRequest.From.Name),
                };

                message.SetTemplateId(templatedEmailRequest.TemplateNameOrId);
                message.SetTemplateData(templatedEmailRequest.TemplateData);

                templatedEmailRequest.To.ForEach(emailAddress =>
                {
                    message.AddTo(new SendGrid.Helpers.Mail.EmailAddress(emailAddress.Email, emailAddress.Name));
                });

                if (templatedEmailRequest.Attachments != null && templatedEmailRequest.Attachments.Any())
                {
                    foreach (var item in templatedEmailRequest.Attachments)
                    {
                        message.AddAttachment(item.FileName, Convert.ToBase64String(item.Content), item.ContentType);
                    }
                }

                await sendGridClient.SendEmailAsync(message);
            }
            catch (SendGridInternalException ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

    }
}
