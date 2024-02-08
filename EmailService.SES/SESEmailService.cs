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
                throw;
            }
        }

        /// <summary>
        /// Creates a raw email message based on the provided Mail object.
        /// </summary>
        /// <param name="mail">The Mail object containing email details such as sender, recipients, subject, body, and attachments.</param>
        /// <returns>A MemoryStream containing the raw email message.</returns>
        private static MemoryStream CreateRawMessage(Mail mail)
        {
            try
            {
                using (var message = new MimeMessage())
                {
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

                    using (var stream = new MemoryStream())
                    {
                        message.WriteTo(stream);
                        stream.Position = 0;
                        return stream;
                    }
                }
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
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        /// <summary>
        /// Retrieves the content of an email template asynchronously from Amazon SES based on the provided template name.
        /// </summary>
        /// <param name="templateName">The name of the template to retrieve.</param>
        /// <param name="client">The AmazonSimpleEmailServiceClient used to communicate with Amazon SES.</param>
        /// <param name="templatedEmailRequest">The TemplatedEmailRequest containing data to be substituted into the template.</param>
        /// <returns>A Task representing the asynchronous operation. The task result contains the content of the email template with dynamic data replaced.</returns>
        private async Task<string> GetEmailTemplateContentAsync(string templateName, AmazonSimpleEmailServiceClient client, TemplatedEmailRequest templatedEmailRequest)
        {
            var request = new GetTemplateRequest
            {
                TemplateName = templateName
            };

            var response = await client.GetTemplateAsync(request);

            string templateContent = response.Template.HtmlPart;

            // Use Handlebars.Net for dynamic template rendering
            var template = Handlebars.Compile(templateContent);
            var replacedContent = template(templatedEmailRequest.TemplateData);

            return replacedContent;
        }

    }
}
