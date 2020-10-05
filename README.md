# EmailService
Generic Email service implementation for sending emails via different Email providers (SES, SendGrid etc.)

# Usage

```
private IEmailService _emailService;

public async Task MyMethod()
{
    var mailMessage = new Mail();
    mail.From = "abc@pqr.com";
    mail.To = new List<string>(){"xyz@domain.com", "lmn@domain.com"};
    mail.CC = new List<string>(){"xyz1@domain.com", "lmn1@domain.com"};
    mail.BCC = new List<string>(){"xyz2@domain.com", "lmn2@domain.com"};
    mail.Subject = "Test subject";
    mail.Body = "Test body";
    mail.IsBodyHTML = true;
    await _emailService.SendEmailAsync(mailMessage);
}
```

# SES

Add nuget package

```
Promact.EmailService.SES
```

## ASP.NET Core projects

Add below in `Startup.cs` inside `ConfigureServices` method

```
services.AddSESEmailService(options =>
{
    options.AccessKeyId = Configuration.GetSection("AWS:AccessKeyId").Value;
    options.SecretAccessKey = Configuration.GetSection("AWS:SecretAccessKey").Value;
    options.Region = Configuration.GetSection("AWS:Region").Value;
});
```

Add relevant appsettings.json values

```
"AWS": {
    "AccessKeyID": "",
    "SecretAccessKey": "",
    "Region":  ""
}
```

Inject `IEmailService` in class constructor from where you want to send emails

```
public MyClass(IEmailService emailService)
{
...
}
```

## Console or other type of applications

Create new object of SESEmailService passing relevant configuration values

```
var emailService = new SESEmailService(new SESOptions(){ });
```

# SendGrid

Add nuget package

```
Promact.EmailService.SendGrid
```

## ASP.NET Core projects

Add below in `Startup.cs` inside `ConfigureServices` method

```
services.AddSendGridEmailService(options =>
{
    options.APIKey = Configuration.GetSection("SendGrid:APIKey").Value;
});
```

Add relevant appsettings.json values

```
"SendGrid": {
    "APIKey": ""
}
```

Inject `IEmailService` in class constructor from where you want to send emails

```
public MyClass(IEmailService emailService)
{
...
}
```

## Console or other type of applications

Create new object of SendGridEmailService passing relevant configuration values

```
var emailService = new SendGridEmailService(new SendGridOptions(){ });
```

#TODO
- Add attachment support
- Add Generic SMTP Email service support (Mailkit/System.NET)
- Setup Github Actions
