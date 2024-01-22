using System.Collections.Generic;
using System.Net.Mail;

namespace EmailService
{
    public class EMail
    {
        public EMail(List<EmailAddress> to, EmailAddress from)
        {
            To = to;
            From = from;
          
        }
        public EmailAddress From { get; set; }

        public List<EmailAddress> To { get; set; }

        public List<AttachmentData>? Attachments { get; set; }



    }

    public class Mail : EMail
    {
 

        public Mail(List<EmailAddress> to, EmailAddress from, string subject, string body) : base(to, from)
        {
            Body = body;
            Subject = subject;
        }

        /// <summary>
        /// HTML of email to be sent.
        /// </summary>
        public string Body { get; set; }

        public string Subject { get; set; }
    }

    public class TemplatedEmailRequest : EMail
    {
        public TemplatedEmailRequest(List<EmailAddress> to, EmailAddress from, string templateNameOrId, dynamic templateData) : base(to, from)
        {
            TemplateNameOrId = templateNameOrId;
            TemplateData = templateData;
        }

        /// <summary>
        /// In case of AWSSES, it will hold template name.
        /// For SendGrid, it will hold template Id. You can find template id
        /// from https://mc.sendgrid.com/dynamic-templates in sendgrid.
        /// </summary>
        public string TemplateNameOrId { get; set; }

        /// <summary>
        /// In case of AWSSES, it will hold serialized object.
        /// In case of SendGrid, it will hold proper object format.
        /// </summary>
        public dynamic TemplateData { get; set; }

    }
    public class EmailAddress
    {
        public EmailAddress(string email, string name)
        {
            Email = email;
            Name = name;
        }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
