namespace TelegramSender.Domain;

public class TelegramMessage
{
    public string ChatId { get; private set; }
    public string Text { get; private set; }

    public TelegramMessage(string chatId, string text)
    {
        ChatId = chatId;
        Text = text;
    }
}
