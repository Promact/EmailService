using EmailService;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace SESEmailService
{
    public static class SESEmailServiceExtensions
    {
        public static IServiceCollection AddSESEmailService(this IServiceCollection serviceCollection, Action<SESOptions> options)
        {
            serviceCollection.AddScoped<IEmailService, SESEmailService>();

            serviceCollection.Configure(options);

            return serviceCollection;
        }
    }
}
