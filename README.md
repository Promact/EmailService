# EmailService
Generic Email service implementation for sending emails via different Email providers (SES, SendGrid etc.)

# Usage

```
private IEmailService _emailService;

public async Task MyMethod()
{
    // The mail which we are using should be verified in Service Provider whichever you are using (SES, SendGrid etc.)
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
    mail.Attachments.Add(new AttachmentData
    {
        Content = System.IO.File.ReadAllBytes("path/to/file.txt"),
        FileName = "FileName",
        ContentType = "FileType"
    });

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
    templatedEmailRequest.Attachments.Add(new AttachmentData
    {
        Content = System.IO.File.ReadAllBytes("path/to/file.txt"),
        FileName = "FileName",
        ContentType = "FileType"
    });

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

#TODO
- Add Generic SMTP Email service support (Mailkit/System.NET)
- Setup Github Actions
