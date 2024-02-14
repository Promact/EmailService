using EmailService;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MimeKit;
using ContentType = MimeKit.ContentType;
using MailKit.Security;
using MailKit.Net.Smtp;
using System.Net;

namespace SMTPEmailService
{
    public class SMTPEmailService : IEmailService
    {
        private readonly SMTPOptions _smtpOptions;

        public SMTPEmailService(SMTPOptions smtpOptions)
        {
            _smtpOptions = smtpOptions;
        }

        public SMTPEmailService(IOptions<SMTPOptions> smtpOptions)
        {
            _smtpOptions = smtpOptions.Value;
        }
        public async Task SendEmailAsync(Email email)
        {
            try
            {
                using (var mimeMessage = new MimeMessage())
                {
                    mimeMessage.From.Add(new MailboxAddress(email.From.Name, email.From.Email));
                    mimeMessage.To.AddRange(email.To.Select(x => new MailboxAddress(x.Name, x.Email)));

                    if (email.CC != null && email.CC.Any())
                    {
                        mimeMessage.Cc.AddRange(email.CC.Select(x => new MailboxAddress(x.Name, x.Email)));
                    }

                    // Add BCC recipients if available
                    if (email.BCC != null && email.BCC.Any())
                    {
                        mimeMessage.Bcc.AddRange(email.BCC.Select(x => new MailboxAddress(x.Name, x.Email)));
                    }

                    mimeMessage.Subject = email.Subject;

                    var builder = new BodyBuilder();
                    if (email.IsBodyHTML)
                    {
                        builder.HtmlBody = email.Body;
                    }
                    else
                    {
                        builder.TextBody = email.Body;
                    }

                    if (email.Attachments != null && email.Attachments.Any())
                    {
                        foreach (var attachment in email.Attachments)
                        {
                            builder.Attachments.Add(attachment.FileName, attachment.Content, ContentType.Parse(attachment.ContentType));
                        }
                    }

                    mimeMessage.Body = builder.ToMessageBody();

                    using (var client = new SmtpClient())
                    {
                        await client.ConnectAsync(_smtpOptions.Host, _smtpOptions.Port, SecureSocketOptions.StartTls);

                        // Authenticate
                        await client.AuthenticateAsync(new NetworkCredential(_smtpOptions.UserName, _smtpOptions.Password));

                        // Send the message
                        await client.SendAsync(mimeMessage);

                        await client.DisconnectAsync(true);
                    }
                }
            }
            catch (Exception ex)
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
