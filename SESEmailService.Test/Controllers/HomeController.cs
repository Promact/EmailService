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

            var mail = new Mail(
                    to: new List<EmailAddress> { new EmailAddress("agrawalprakhar893@gmail.com", "Recipient Name") },
                    from: new EmailAddress("prakharagrawal@promactinfo.com", "Sender Name"),
                    subject: "Greeting",
                    body: "Hi Prakhar "
                );

            _emailService.SendEmailAsync(mail).Wait();




            var templatedEmailRequest = new TemplatedEmailRequest(
                 to: new List<EmailAddress> { new EmailAddress("prakharagrawal@promactinfo.com", "Prakhar Agrawal") },
                 from: new EmailAddress("agrawalprakhar893@gmail.com", "Prakhar"),
                 templateNameOrId: "HelloMail", // Replace with your actual template name or ID
                 templateData: new { name = "Value1", favoriteanimal = "Value2" } // Replace with your actual template data
             );

            _emailService.SendTemplatedEmailAsync(templatedEmailRequest);

            _logger.LogInformation("Templated email sent successfully from the constructor. Hooray!");
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
