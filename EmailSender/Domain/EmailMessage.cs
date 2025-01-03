namespace EmailSender.Domain;

public class EmailMessage
{
    public EmailMessage(string subject, string body, string recipientEmail)
    {
        Subject = subject;
        Body = body;
        RecipientEmail = recipientEmail;
    }

    public string Subject { get; set; }
    public string Body { get; set; }
    public string RecipientEmail { get; set; }
}
