namespace EmailSender.Domain;

public class EmailMessage
{
    public EmailMessage(string subject, string body, string recipientEmail)
    {
        this.subject = subject;
        this.body = body;
        this.recipientEmail = recipientEmail;
    }

    public string subject { get; set; }
    public string body { get; set; }
    public string recipientEmail { get; set; }
}
