using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using EmailService;
using Microsoft.Extensions.Options;
using MimeKit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
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
                    var sendRequest = new SendEmailRequest
                    {
                        Source = $"{mail.From.Name} <{mail.From.Email}>",
                        Message = new Message
                        {
                            Body = new Body { Html = new Content { Data = mail.Body } },
                            Subject = new Content { Data = mail.Subject },
                        },
                        Destination = new Destination { ToAddresses = mail.To.Select(x => x.Email).ToList() }
                    };

                    if (mail.Attachments != null && mail.Attachments.Any())
                    {
                        var messageWithAttachments = new SendRawEmailRequest
                        {
                            Source = sendRequest.Source,
                            Destinations = sendRequest.Destination.ToAddresses,
                            RawMessage = new RawMessage
                            {
                                Data = CreateRawMessage(mail)
                            }
                        };

                        await client.SendRawEmailAsync(messageWithAttachments);
                    }
                    else
                    {
                        sendRequest.Destination.ToAddresses.AddRange(mail.To.Select(x => x.Email));
                        await client.SendEmailAsync(sendRequest);
                    }
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
                message.Subject = mail.Subject;

                var builder = new BodyBuilder();
                builder.HtmlBody = mail.Body;

                foreach (var attachment in mail.Attachments)
                {
                    builder.Attachments.Add(attachment.FileName, attachment.Content, ContentType.Parse(attachment.ContentType));
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
                    var sendRequest = new SendTemplatedEmailRequest
                    {
                        Source = $"{templatedEmailRequest.From.Name} <{templatedEmailRequest.From.Email}>",
                        Template = templatedEmailRequest.TemplateNameOrId,
                        TemplateData = JsonConvert.SerializeObject(templatedEmailRequest.TemplateData),
                        Destination = new Destination { ToAddresses = new List<string>() }
                    };

                    sendRequest.Destination.ToAddresses.AddRange(templatedEmailRequest.To.Select(x => x.Email));
                    Console.WriteLine("Hi");
                    await client.SendTemplatedEmailAsync(sendRequest);
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

    }
}
