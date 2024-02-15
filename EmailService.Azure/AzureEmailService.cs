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

        public async Task SendEmailAsync(Email email)
        {
            EmailClient client = null;
            try
            {
                client = new EmailClient(_azureOptions.ConnectionString);
                {
                    var sender = $"<{email.From.Email}>";

                    var subject = email.Subject;

                    var emailContent = new EmailContent(subject);

                    if (email.IsBodyHTML)
                    {
                        emailContent.Html = email.Body;
                    }
                    else
                    {
                        emailContent.PlainText = email.Body;
                    }

                    var toRecipients = new List<Azure.Communication.Email.EmailAddress>();
                    foreach (var to in email.To)
                    {
                        toRecipients.Add(new Azure.Communication.Email.EmailAddress(to.Email, to.Name));
                    }

                    var ccRecipients = new List<Azure.Communication.Email.EmailAddress>();
                    if (email.CC != null)
                    {
                        foreach (var cc in email.CC)
                        {
                            ccRecipients.Add(new Azure.Communication.Email.EmailAddress(cc.Email, cc.Name));
                        }
                    }

                    var bccRecipients = new List<Azure.Communication.Email.EmailAddress>();
                    if (email.BCC != null)
                    {
                        foreach (var bcc in email.BCC)
                        {
                            bccRecipients.Add(new Azure.Communication.Email.EmailAddress(bcc.Email, bcc.Name));
                        }
                    }

                    var emailRecipients = new EmailRecipients(toRecipients, ccRecipients, bccRecipients);

                    var emailMessage = new EmailMessage(sender, emailRecipients, emailContent);


                    // Add attachments
                    if (email.Attachments != null && email.Attachments.Any())
                    {
                        foreach (var attachment in email.Attachments)
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