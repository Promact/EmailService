using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using EmailService;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
            using (var client = new AmazonSimpleEmailServiceClient(_sesOptions.AccessKeyId, _sesOptions.SecretAccessKey, RegionEndpoint.GetBySystemName(_sesOptions.Region)))
            {
                var sendRequest = new SendEmailRequest
                {
                    Source = $"{mail.From.Name} <{mail.From.Email}>",
                    Message = new Message
                    {
                        Body = new Body { Html = new Content { Data = mail.Body } },
                        Subject = new Content { Data = mail.Subject },
                        Attachments = new List<Attachment> { attachment }
                    },
                    Destination = new Destination { ToAddresses = new List<string>() }
                };
                sendRequest.Destination.ToAddresses.AddRange(mail.To.Select(x => x.Email));



                await client.SendEmailAsync(sendRequest);
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
                // Handle Amazon SES specific exceptions
                // Log or handle the exception as needed
                Console.WriteLine($"Amazon SES Exception: {ex.Message}");
                throw; // You may choose to handle, log, or rethrow the exception
            }
            catch (Exception ex)
            {
                // Handle general exceptions
                // Log or handle the exception as needed
                Console.WriteLine($"Exception: {ex.Message}");
                throw; // You may choose to handle, log, or rethrow the exception
            }
        }

    }
}
