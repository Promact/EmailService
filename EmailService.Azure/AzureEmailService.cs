using Azure;
using Azure.Communication.Email;
using EmailService;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace AzureEmailService
{
    public class AzureEmailService : IEmailService
    {
        private readonly AzureOptions _azureOptions;

        public AzureEmailService(AzureOptions azureOptions)
        {
            _azureOptions = azureOptions;
        }

        public AzureEmailService(IOptions<AzureOptions> azureOptions)
        {
            _azureOptions = azureOptions.Value;
        }

        public async Task SendEmailAsync(Mail mail)
        {
            EmailClient client = null;
            try
            {
                client = new EmailClient(_azureOptions.ConnectionString);
                {
                    var sender = $"<{mail.From.Email}>";

                    var subject = mail.Subject;

                    var emailContent = new EmailContent(subject);

                    if (mail.IsBodyHTML)
                    {
                        emailContent.Html = mail.Body;
                    }
                    else
                    {
                        emailContent.PlainText = mail.Body;
                    }

                    var toRecipients = new List<Azure.Communication.Email.EmailAddress>();
                    foreach (var to in mail.To)
                    {
                        toRecipients.Add(new Azure.Communication.Email.EmailAddress(to.Email, to.Name));
                    }

                    var ccRecipients = new List<Azure.Communication.Email.EmailAddress>();
                    if (mail.CC != null)
                    {
                        foreach (var cc in mail.CC)
                        {
                            ccRecipients.Add(new Azure.Communication.Email.EmailAddress(cc.Email, cc.Name));
                        }
                    }

                    var bccRecipients = new List<Azure.Communication.Email.EmailAddress>();
                    if (mail.BCC != null)
                    {
                        foreach (var bcc in mail.BCC)
                        {
                            bccRecipients.Add(new Azure.Communication.Email.EmailAddress(bcc.Email, bcc.Name));
                        }
                    }

                    var emailRecipients = new EmailRecipients(toRecipients, ccRecipients, bccRecipients);

                    var emailMessage = new EmailMessage(sender, emailRecipients, emailContent);


                    // Add attachments
                    if (mail.Attachments != null && mail.Attachments.Any())
                    {
                        foreach (var attachment in mail.Attachments)
                        {
                            var binaryData = new BinaryData(attachment.Content);
                            var emailAttachment = new EmailAttachment(attachment.FileName, attachment.ContentType, binaryData);
                            emailMessage.Attachments.Add(emailAttachment);
                        }
                    }
                    await client.SendAsync(WaitUntil.Completed, emailMessage);

                }
            }
            catch (RequestFailedException ex)
            {
                throw;
            }
        }


        public Task SendTemplatedEmailAsync(TemplatedEmailRequest templatedEmailRequest)
        {
            throw new NotImplementedException();
        }
    }
}