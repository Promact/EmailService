using EmailService;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AzureEmailService
{
    public static class AzureEmailServiceExtensions
    {
        public static IServiceCollection AddAzureEmailService(this IServiceCollection serviceCollection, Action<AzureOptions> options)
        {
            serviceCollection.AddScoped<IEmailService, AzureEmailService>();

            serviceCollection.Configure(options);

            return serviceCollection;
        }
    }
}
