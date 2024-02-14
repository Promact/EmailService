using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmailService
{
    public interface IEmailService
    {
        /// <summary>
        /// Sends an email asynchronously using the provided Mail object.
        /// </summary>
        /// <param name="email">The Mail object containing email details such as sender, recipients, subject, body, and attachments.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        Task SendEmailAsync(Email email);


        /// <summary>
        /// Sends a templated email asynchronously using the provided TemplatedEmailRequest.
        /// </summary>
        /// <param name="templatedEmailRequest">The TemplatedEmailRequest containing details such as template name, template data, and email parameters.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        Task SendTemplatedEmailAsync(TemplatedEmailRequest templatedEmailRequest);
    }
}
