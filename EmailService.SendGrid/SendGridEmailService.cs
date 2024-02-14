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

        public async Task SendEmailAsync(Email email)
        {
            try
            {
                var sendGridClient = new SendGridClient(_sendGridOptions.APIKey);

                var sendGridMessage = new SendGridMessage
                {
                    From = new SendGrid.Helpers.Mail.EmailAddress(email.From.Email, email.From.Name),
                    Subject = email.Subject,
                };

                if (email.IsBodyHTML)
                {
                    sendGridMessage.HtmlContent = email.Body;
                }

                else
                {
                    sendGridMessage.PlainTextContent = email.Body;
                }

                sendGridMessage.AddTos(email.To.Select(x => new SendGrid.Helpers.Mail.EmailAddress(x.Email, x.Name)).ToList());

                // Add CC and BCC recipients
                if (email.CC != null && email.CC.Any())
                {
                    sendGridMessage.AddCcs(email.CC.Select(x => new SendGrid.Helpers.Mail.EmailAddress(x.Email, x.Name)).ToList());
                }

                if (email.BCC != null && email.BCC.Any())
                {
                    sendGridMessage.AddBccs(email.BCC.Select(x => new SendGrid.Helpers.Mail.EmailAddress(x.Email, x.Name)).ToList());
                }

                // Additional options can be set here, like attachments, headers, etc.
                if (email.Attachments != null && email.Attachments.Any())
                {
                    foreach (var item in email.Attachments)
                    {
                        sendGridMessage.AddAttachment(item.FileName, Convert.ToBase64String(item.Content), item.ContentType);
                    }
                }

                await sendGridClient.SendEmailAsync(sendGridMessage);
            }
            catch (SendGridInternalException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
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

                // Add CC and BCC recipients
                if (templatedEmailRequest.CC != null && templatedEmailRequest.CC.Any())
                {
                    message.AddCcs(templatedEmailRequest.CC.Select(x => new SendGrid.Helpers.Mail.EmailAddress(x.Email, x.Name)).ToList());
                }

                if (templatedEmailRequest.BCC != null && templatedEmailRequest.BCC.Any())
                {
                    message.AddBccs(templatedEmailRequest.BCC.Select(x => new SendGrid.Helpers.Mail.EmailAddress(x.Email, x.Name)).ToList());
                }


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
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
