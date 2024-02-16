using Azure.Communication.Email;
using EmailService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace AzureEmailService
{
    public static class AzureEmailServiceExtensions
    {
        public static IServiceCollection AddAzureEmailService(this IServiceCollection serviceCollection, Action<AzureOptions> options)
        {
    
            serviceCollection.AddScoped<IEmailService, AzureEmailService>();
            serviceCollection.Configure(options);
            serviceCollection.AddScoped<EmailClient>(provider =>
            {
                var azureOptions = provider.GetRequiredService<IOptions<AzureOptions>>().Value;
                var connectionString = azureOptions.ConnectionString;

                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new Exception("Azure connection string is empty or null.");
                }

                return new EmailClient(connectionString);
            });


            return serviceCollection;
        }
    }
}
