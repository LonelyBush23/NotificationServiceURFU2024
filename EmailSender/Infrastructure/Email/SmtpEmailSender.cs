using System;
using EmailSender.Domain;
using System.Net;
using System.Net.Mail;

namespace EmailSender.Infrastructure.Email;

public class SmtpEmailSender : IEmailSender
{
    private readonly SmtpClient _smtpClient;
    private readonly string _senderEmail;

    public SmtpEmailSender(IConfiguration configuration)
    {
        var host = configuration["SMTP:Host"];
        var port = configuration.GetValue<int>("SMTP:Post");
        var username = configuration["SMTP:Username"];
        var password = configuration["SMTP:Password"];

        if (host == null || username == null || password == null)
        {
            throw new ArgumentNullException(nameof(configuration), "SMTP credentials is not found in configuration file");
        }

        _senderEmail = username;

        _smtpClient = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(username, password),
            EnableSsl = true
        };
    }

    public async Task SendAsync(EmailMessage message)
    {
        var mailMessage = new MailMessage(_senderEmail, message.RecipientEmail, message.Subject, message.Body);
        await _smtpClient.SendMailAsync(mailMessage);
    }
}
