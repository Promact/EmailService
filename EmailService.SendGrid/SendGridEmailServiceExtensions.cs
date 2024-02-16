using EmailService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SendGrid;
using System;

namespace SendGridEmailService
{
    public static class SendGridEmailServiceExtensions
    {
        public static IServiceCollection AddSendGridEmailService(this IServiceCollection serviceCollection, Action<SendGridOptions> options)
        {
            serviceCollection.AddScoped<IEmailService, SendGridEmailService>();

            serviceCollection.Configure(options);
            // Configure SendGrid client as a singleton
            serviceCollection.AddSingleton<ISendGridClient>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var config = configuration.GetSection("SendGrid");
                var apiKey = config["ApiKey"];
                if (string.IsNullOrEmpty(apiKey))
                    throw new Exception($"Invalid SendGridConfig API Key: {apiKey}");

                return new SendGridClient(apiKey);
            });

            return serviceCollection;
        }
    }
}
