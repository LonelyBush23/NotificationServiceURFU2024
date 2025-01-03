namespace TelegramSender.Domain;

public class TelegramMessage
{
    public string chatId { get; private set; }
    public string text { get; private set; }

    public TelegramMessage(string chatId, string text)
    {
        this.chatId = chatId;
        this.text = text;
    }
}
