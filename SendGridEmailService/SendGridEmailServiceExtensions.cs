using EmailService;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace SendGridEmailService
{
    public static class SendGridEmailServiceExtensions
    {
        public static IServiceCollection AddSendGridEmailService(this IServiceCollection serviceCollection, Action<SendGridOptions> options)
        {
            serviceCollection.AddScoped<IEmailService, SendGridEmailService>();

            serviceCollection.Configure(options);

            return serviceCollection;
        }
    }
}
