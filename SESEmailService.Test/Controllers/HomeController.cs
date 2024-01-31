using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EmailService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SESEmailService.Test.Models;

namespace SESEmailService.Test.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmailService _emailService;

        public HomeController(ILogger<HomeController> logger, IEmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;

            // The mail which we are using should be verified in AWS Identities
            var mail = new Mail(
                 to: new List<EmailAddress> { new EmailAddress("ReceiverEmail", "Receiver Name") },
                 from: new EmailAddress("SenderEmail", "Sender Name"),
                 cc: new List<EmailAddress> { new EmailAddress("ReceiverEmail", "Receiver Name") },
                 bcc: new List<EmailAddress> { new EmailAddress("ReceiverEmail", "Receiver Name") },
                 subject: "Subject",
                 body: "Content",
                 isBodyHtml: true
                );

            // Add attachments
            mail.Attachments.Add(new AttachmentData(
                content: System.IO.File.ReadAllBytes("path/to/file.txt"),
                fileName: "FileName",
                contentType: "FileType"
            ));

            _emailService.SendEmailAsync(mail);


            //sending an email with Template
            var templatedEmailRequest = new TemplatedEmailRequest(
                 to: new List<EmailAddress> { new EmailAddress("ReceiverEmail", "Receiver Name") },
                 from: new EmailAddress("SenderEmail", "Sender Name"),
                 cc: new List<EmailAddress> { new EmailAddress("ReceiverEmail", "Receiver Name") },
                 bcc: new List<EmailAddress> { new EmailAddress("ReceiverEmail", "Receiver Name") },
                 templateNameOrId: "TemplateID", // Replace with your actual template name or ID
                 templateData: new { name = "Value1" }, // Replace with your actual template data            
                 subject: ""
             );

            // Add attachments
            templatedEmailRequest.Attachments.Add(new AttachmentData(
                content: System.IO.File.ReadAllBytes("path/to/file.txt"),
                fileName: "FileName",
                contentType: "FileType"
            ));

            _emailService.SendTemplatedEmailAsync(templatedEmailRequest);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
