using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmailService;
using Microsoft.Extensions.DependencyInjection;

namespace SMTPEmailService
{
    public static class SMTPEmailServiceExtensions
    {
        public static IServiceCollection AddSMTPEmailService(this IServiceCollection serviceCollection, Action<SMTPOptions> options)
        {
            serviceCollection.AddScoped<IEmailService, SMTPEmailService>();

            serviceCollection.Configure(options);

            return serviceCollection;
        }
    }
}
