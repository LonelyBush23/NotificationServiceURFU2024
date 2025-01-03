namespace TelegramSender.Domain;

public interface ITelegramSender
{
    Task SendMessageAsync(TelegramMessage message);
}
