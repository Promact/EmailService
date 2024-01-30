using EmailService;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace SMTPEmailService.Test.Controllers
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
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


    }
}