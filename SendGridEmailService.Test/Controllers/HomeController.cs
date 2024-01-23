using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EmailService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SendGridEmailService.Test.Models;

namespace SendGridEmailService.Test.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmailService _emailService;

        public HomeController(ILogger<HomeController> logger, IEmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;

            var mail = new Mail(
                 to: new List<EmailAddress> { new EmailAddress("agrawalprakhar893@gmail.com", "Prakhar Agrawal") },//the mail which we are using should be verified in AWS Identites
                 from: new EmailAddress("prakharagrawal@promactinfo.com", "Rishabh Dev"),
                 subject: "Greeting",
                 body: "Hi Prakhar Bhai"
             );

            // Add attachments
            mail.Attachments.Add(new AttachmentData(
               content: System.IO.File.ReadAllBytes("C:/Users/admin/Downloads/Progress Updated @12_01 - Sheet1.pdf"),
                fileName: "Progress Updated @12_01 - Sheet1.pdf",
                contentType: "application/pdf"
            ));

            _emailService.SendEmailAsync(mail);

           // sending an email with Template
            var templatedEmailRequest = new TemplatedEmailRequest(
                 to: new List<EmailAddress> { new EmailAddress("agrawalprakhar893@gmail.com", "Prakhar Agrawal") },
                 from: new EmailAddress("prakharagrawal@promactinfo.com", "Prakhar"),
                 templateNameOrId: "d-1ee30ec9b1f1495a8c0f8edf4cd8c7b4", // Replace with your actual template name or ID
                 templateData: new { name = "Value1" } // Replace with your actual template data
             );

            templatedEmailRequest.Attachments.Add(new AttachmentData(
           content: System.IO.File.ReadAllBytes("C:/Users/admin/Downloads/Progress Updated @12_01 - Sheet1.pdf"),
            fileName: "Progress Updated @12_01 - Sheet1.pdf",
            contentType: "application/pdf"
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
