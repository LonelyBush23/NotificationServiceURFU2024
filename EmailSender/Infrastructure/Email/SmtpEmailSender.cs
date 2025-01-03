using System;
using EmailSender.Domain;
using System.Net;
using System.Net.Mail;

namespace EmailSender.Infrastructure.Email;

public class SmtpEmailSender : IEmailSender
{
    private readonly SmtpClient _smtpClient;
    private readonly string _senderEmail;

    public SmtpEmailSender()
    {
        var host = Environment.GetEnvironmentVariable("SMTP_HOST");
        int port = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "0");
        var username = Environment.GetEnvironmentVariable("SMTP_USERNAME");
        var password = Environment.GetEnvironmentVariable("SMTP_PASSWORD");

        if (host == null || username == null || password == null)
        {
            throw new ArgumentNullException("SMTP credentials is not found in configuration file");
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
