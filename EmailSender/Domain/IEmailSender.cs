namespace EmailSender.Domain;

public interface IEmailSender
{
    Task SendAsync(EmailMessage message);
}
