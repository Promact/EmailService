# EmailService
Generic Email service implementation for sending emails via different Email providers (SES, SendGrid,SMTP,Azure etc.)

# Usage

```
private IEmailService _emailService;

public async Task MyMethod()
{
    // The mail which we are using should be verified in Service Provider whichever you are using (SES, SendGrid,SMTP etc.)
    //In Case Of Azure Your EmailDomain Should be Verified And Connected To Your  Email Communications Services

    var mail = new Mail(
        to: new List<EmailAddress>
        {
            new EmailAddress("xyz@domain.com", "Receiver Name"),
            new EmailAddress("lmn@domain.com", "Receiver Name")
        },
        from: new EmailAddress("abc@pqr.com", "Sender Name"),
        cc: new List<EmailAddress>
        {
            new EmailAddress("xyz1@domain.com", "Receiver Name"),
            new EmailAddress("lmn1@domain.com", "Receiver Name")
        },
        bcc: new List<EmailAddress>
        {
            new EmailAddress("xyz2@domain.com", "Receiver Name"),
            new EmailAddress("lmn2@domain.com", "Receiver Name")
        },
        subject: "Test subject",
        body: "Test body",
        isBodyHtml: true
    );

    // Add attachments. You can Attach Multiple Attachment
    mail.Attachments.Add(new AttachmentData(
        content: System.IO.File.ReadAllBytes("path/to/file.txt"),
        fileName: "FileName",
        contentType: "FileType"
    ));

    await _emailService.SendEmailAsync(mail);
}

public async Task SendTemplatedEmail()
{
    var templatedEmailRequest = new TemplatedEmailRequest(
        to: new List<EmailAddress>
        {
            new EmailAddress("xyz@domain.com", "Receiver Name"),
            new EmailAddress("lmn@domain.com", "Receiver Name")
        },
        from: new EmailAddress("abc@pqr.com", "Sender Name"),
        cc: new List<EmailAddress>
        {
            new EmailAddress("xyz1@domain.com", "Receiver Name"),
            new EmailAddress("lmn1@domain.com", "Receiver Name")
        },
        bcc: new List<EmailAddress>
        {
            new EmailAddress("xyz2@domain.com", "Receiver Name"),
            new EmailAddress("lmn2@domain.com", "Receiver Name")
        },
        templateNameOrId: "TemplateID", // Replace with your actual template name or ID
        templateData: new { name = "Value1" }, // Replace with your actual template data            
        subject: "Subject"
    );

    // Add attachments
    templatedEmailRequest.Attachments.Add(new AttachmentData(
        content: System.IO.File.ReadAllBytes("path/to/file.txt"),
        fileName: "FileName",
        contentType: "FileType"
    ));

    await _emailService.SendTemplatedEmailAsync(templatedEmailRequest);
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

# SMTP

Add nuget package

```
Promact.EmailService.SMTP
```

## ASP.NET Core projects

Add below in `Startup.cs` inside `ConfigureServices` method

```
 services.AddSMTPEmailService(options =>
 {
     options.Host = configuration.GetSection("SMTP:Host").Value;
     options.Port = int.Parse(configuration.GetSection("SMTP:Port").Value);
     options.UserName = configuration.GetSection("SMTP:UserName").Value;
     options.Password = configuration.GetSection("SMTP:Password").Value;
 });
```

Add relevant appsettings.json values

```
"SMTP": {
  "Host": "",
  "Port":,
  "UserName": "",
  "Password": ""
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

Create new object of SMTPEmailService passing relevant configuration values

```
var emailService = new SMTPEmailService(new SMTPOptions(){ });
```

# Azure

Add nuget package

```
Promact.EmailService.Azure
```

## ASP.NET Core projects

Add below in `Startup.cs` inside `ConfigureServices` method

```
services.AddAzureEmailService(options =>
{
    options.ConnectionString= Configuration.GetSection("Azure:ConnectionString").Value;
});
```

Add relevant appsettings.json values

```
"Azure": {
  "ConnectionString": ""
},
```

Inject `IEmailService` in class constructor from where you want to send emails

```
public MyClass(IEmailService emailService)
{
...
}
```

## Console or other type of applications

Create new object of AzureEmailService passing relevant configuration values

```
var emailService = new AzureEmailService(new AzureOptions(){ });
```
