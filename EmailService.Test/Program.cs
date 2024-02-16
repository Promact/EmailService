using SESEmailService;
using SMTPEmailService;
using SendGridEmailService;
using AzureEmailService;
using Microsoft.Extensions.Configuration;

namespace EmailService.Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Add services to the container.
            ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }

        public static void ConfigureServices(IServiceCollection services, IConfiguration Configuration)
        {
            services.AddControllersWithViews();

            // Retrieve the value of "EmailServiceType" from configuration
            string emailServiceType = Configuration["EmailServiceType"];

            // Use the value of emailServiceType to configure services accordingly
            if (emailServiceType == "SMTP")
            {
                services.AddSMTPEmailService(options =>
                {
                    options.Host = Configuration.GetSection("SMTP:Host").Value;
                    options.Port = int.Parse(Configuration.GetSection("SMTP:Port").Value);
                    options.UserName = Configuration.GetSection("SMTP:UserName").Value;
                    options.Password = Configuration.GetSection("SMTP:Password").Value;
                });
            }
            else if (emailServiceType == "AWS")
            {
                services.AddSESEmailService(options =>
                {
                    options.AccessKeyId = Configuration.GetSection("AWS:AccessKeyId").Value;
                    options.SecretAccessKey = Configuration.GetSection("AWS:SecretAccessKey").Value;
                    options.Region = Configuration.GetSection("AWS:Region").Value;
                });
            }
            else if (emailServiceType == "SendGrid")
            {
                services.AddSendGridEmailService(options =>
                {
                    options.APIKey = Configuration.GetSection("SendGrid:APIKey").Value;
                });
            }
            else if (emailServiceType == "Azure")
            {
                services.AddAzureEmailService(options =>
                {
                    options.ConnectionString= Configuration.GetSection("Azure:ConnectionString").Value;
                });
            }
            else
            {
                throw new InvalidOperationException("Invalid email service type specified in configuration.");
            }

        }
    }
}