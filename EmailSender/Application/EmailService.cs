using EmailSender.Domain;

namespace EmailSender.Application;

public class EmailService
{
    private readonly IEmailSender _emailSender;

    public EmailService(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    public async Task SendEmailAsync(string subject, string body, string recipientEmail)
    {
        var emailMessage = new EmailMessage(subject, body, recipientEmail);
        await _emailSender.SendAsync(emailMessage);
    }
}
