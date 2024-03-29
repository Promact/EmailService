﻿using EmailService.Test.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EmailService.Test.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmailService _emailService;

        public HomeController(ILogger<HomeController> logger, IEmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;

            // The mail which we are using should be verified in AWS,SMTP,SendGrid etc
            //In Case of Azure Email Domain Should be verified and Connect your Email Communication Service to Your Domain
            var email = new Email(
                 to: new List<EmailAddress> { new EmailAddress("ReceiverEmail", "Receiver Name") },
                 from: new EmailAddress("SenderEmail", "Sender Name"),
                 cc: new List<EmailAddress> { new EmailAddress("ReceiverEmail", "Receiver Name") },
                 bcc: new List<EmailAddress> { new EmailAddress("ReceiverEmail", "Receiver Name") },
                 subject: "Subject",
                 body: "Content",
                 isBodyHtml: true
                );

            // Add attachments
            email.Attachments.Add(new AttachmentData(
                content: System.IO.File.ReadAllBytes("path/to/file.txt"),
                fileName: "FileName",
                contentType: "FileType"
            ));

            _emailService.SendEmailAsync(email);


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