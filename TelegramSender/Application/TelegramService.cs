using TelegramSender.Domain;

namespace TelegramSender.Application;

public class TelegramService
{
    private readonly ITelegramSender _telegramSender;

    public TelegramService(ITelegramSender telegramSender)
    {
        _telegramSender = telegramSender;
    }

    public async Task SendMessageAsync(string chatId, string text)
    {
        var message = new TelegramMessage(chatId, text);
        await _telegramSender.SendMessageAsync(message);
    }
}