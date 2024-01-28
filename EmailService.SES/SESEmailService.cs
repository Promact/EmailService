using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using EmailService;
using HandlebarsDotNet;
using Microsoft.Extensions.Options;
using MimeKit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
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
            try
            {
                using (var client = new AmazonSimpleEmailServiceClient(_sesOptions.AccessKeyId, _sesOptions.SecretAccessKey, RegionEndpoint.GetBySystemName(_sesOptions.Region)))
                {
                    var messageRequest = new SendRawEmailRequest
                    {
                        Source = $"{mail.From.Name} <{mail.From.Email}>",
                        Destinations = mail.To.Select(x => x.Email).ToList()
                            .Concat(mail.CC?.Select(x => x.Email) ?? Enumerable.Empty<string>())
                            .Concat(mail.BCC?.Select(x => x.Email) ?? Enumerable.Empty<string>())
                            .ToList(),
                        RawMessage = new RawMessage
                        {
                            Data = CreateRawMessage(mail)
                        }
                    };

                    await client.SendRawEmailAsync(messageRequest);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while sending the email: {ex.Message}");
            }
        }

        private static MemoryStream CreateRawMessage(Mail mail)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(mail.From.Name, mail.From.Email));
                message.To.AddRange(mail.To.Select(x => new MailboxAddress(x.Name, x.Email)));
                // Add CC recipients if available
                if (mail.CC != null && mail.CC.Any())
                {
                    message.Cc.AddRange(mail.CC.Select(x => new MailboxAddress(x.Name, x.Email)));
                }

                // Add BCC recipients if available
                if (mail.BCC != null && mail.BCC.Any())
                {
                    message.Bcc.AddRange(mail.BCC.Select(x => new MailboxAddress(x.Name, x.Email)));
                }

                message.Subject = mail.Subject;

                var builder = new BodyBuilder();
                if (mail.IsBodyHTML)
                {
                    builder.HtmlBody = mail.Body;
                }
                else
                {
                    builder.TextBody = mail.Body;
                }

                if (mail.Attachments != null && mail.Attachments.Any())
                {
                    foreach (var attachment in mail.Attachments)
                    {
                        builder.Attachments.Add(attachment.FileName, attachment.Content, ContentType.Parse(attachment.ContentType));
                    }
                }

                message.Body = builder.ToMessageBody();

                var stream = new MemoryStream();
                message.WriteTo(stream);
                stream.Position = 0; // Reset the stream position to the beginning

                return stream;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while creating the raw email message: {ex.Message}");
                throw;
            }
        }

        public async Task SendTemplatedEmailAsync(TemplatedEmailRequest templatedEmailRequest)
        {
            try
            {
                using (var client = new AmazonSimpleEmailServiceClient(_sesOptions.AccessKeyId, _sesOptions.SecretAccessKey, RegionEndpoint.GetBySystemName(_sesOptions.Region)))
                {
                    // Get the HTML body of the email template
                    var templateContent = await GetEmailTemplateContentAsync(templatedEmailRequest.TemplateNameOrId, client, templatedEmailRequest);

                    // Create a MIME message
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress(templatedEmailRequest.From.Name, templatedEmailRequest.From.Email));
                    message.To.AddRange(templatedEmailRequest.To.Select(x => new MailboxAddress(x.Name, x.Email)));
                    // Add CC recipients if available
                    if (templatedEmailRequest.CC != null && templatedEmailRequest.CC.Any())
                    {
                        message.Cc.AddRange(templatedEmailRequest.CC.Select(x => new MailboxAddress(x.Name, x.Email)));
                    }

                    // Add BCC recipients if available
                    if (templatedEmailRequest.BCC != null && templatedEmailRequest.BCC.Any())
                    {
                        message.Bcc.AddRange(templatedEmailRequest.BCC.Select(x => new MailboxAddress(x.Name, x.Email)));
                    }
                    message.Subject = templatedEmailRequest.Subject;


                    // Create a body part with the template content
                    var bodyPart = new TextPart("html")
                    {
                        Text = templateContent
                    };

                    // Create attachment parts if attachments are available
                    if (templatedEmailRequest.Attachments != null && templatedEmailRequest.Attachments.Any())
                    {
                        var attachmentParts = templatedEmailRequest.Attachments
                            .Select(attachment => new MimePart
                            {
                                Content = new MimeContent(new MemoryStream(attachment.Content), ContentEncoding.Default),
                                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                                ContentTransferEncoding = ContentEncoding.Base64,
                                Headers = { { "Content-Type", attachment.ContentType } },
                                FileName = attachment.FileName
                            });

                        // Add parts to the MIME message
                        var multipart = new Multipart("mixed");
                        multipart.Add(bodyPart);
                        foreach (var attachmentPart in attachmentParts)
                        {
                            multipart.Add(attachmentPart);
                        }

                        message.Body = multipart;
                    }
                    else
                    {
                        // If no attachments, simply add the body part
                        message.Body = bodyPart;
                    }

                    // Convert the MimeMessage to a stream
                    using (var memoryStream = new MemoryStream())
                    {
                        await message.WriteToAsync(memoryStream);
                        memoryStream.Seek(0, SeekOrigin.Begin);

                        // Set the stream in the send request
                        var sendRequest = new SendRawEmailRequest
                        {
                            RawMessage = new RawMessage
                            {
                                Data = memoryStream
                            }
                        };

                        await client.SendRawEmailAsync(sendRequest);
                    }
                }
            }
            catch (AmazonSimpleEmailServiceException ex)
            {
                Console.WriteLine($"Amazon SES Exception: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                throw;
            }
        }

        private async Task<string> GetEmailTemplateContentAsync(string templateName, AmazonSimpleEmailServiceClient client, TemplatedEmailRequest templatedEmailRequest)
        {
            var request = new GetTemplateRequest
            {
                TemplateName = templateName
            };

            var response = await client.GetTemplateAsync(request);

            string templateContent = response.Template.HtmlPart; // Assuming the template contains an HTML part

            // Use Handlebars.Net for dynamic template rendering
            var template = Handlebars.Compile(templateContent);
            var replacedContent = template(templatedEmailRequest.TemplateData);

            return replacedContent;
        }

    }
}
