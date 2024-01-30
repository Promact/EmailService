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
        public async Task SendEmailAsync(Mail mail)
        {
            try
            {
                var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress(mail.From.Name, mail.From.Email));
                mimeMessage.To.AddRange(mail.To.Select(x => new MailboxAddress(x.Name, x.Email)));

                if (mail.CC != null && mail.CC.Any())
                {
                    mimeMessage.Cc.AddRange(mail.CC.Select(x => new MailboxAddress(x.Name, x.Email)));
                }

                // Add BCC recipients if available
                if (mail.BCC != null && mail.BCC.Any())
                {
                    mimeMessage.Bcc.AddRange(mail.BCC.Select(x => new MailboxAddress(x.Name, x.Email)));
                }

                mimeMessage.Subject = mail.Subject;

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
