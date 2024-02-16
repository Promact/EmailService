using Amazon;
using Amazon.SimpleEmail;
using EmailService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace SESEmailService
{
    public static class SESEmailServiceExtensions
    {
        public static IServiceCollection AddSESEmailService(this IServiceCollection serviceCollection, Action<SESOptions> options)
        {
            serviceCollection.AddScoped<IEmailService, SESEmailService>();

            serviceCollection.Configure(options);

            // Configure AWS Simple Email Service client as a singleton
            serviceCollection.AddSingleton<IAmazonSimpleEmailService>(sp =>
            {
                var config = sp.GetRequiredService<IOptions<SESOptions>>().Value;
                var region = config.Region;
                if (string.IsNullOrEmpty(region))
                {
                    throw new Exception($"Invalid SES region: {region}");
                }
                var regionEndpoint = RegionEndpoint.GetBySystemName(region);
                return new AmazonSimpleEmailServiceClient(regionEndpoint);
            });


            return serviceCollection;
        }
    }
}
