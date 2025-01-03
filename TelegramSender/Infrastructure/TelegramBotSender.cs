using TelegramSender.Domain;
using Telegram.Bot;

namespace TelegramSender.Application;

public class TelegramBotSender : ITelegramSender
{
    private readonly TelegramBotClient _botClient;

    public TelegramBotSender()
    {
        var token = Environment.GetEnvironmentVariable("TELEGRAM_TOKEN");

        if (token == null)
        {
            throw new ArgumentNullException("SMTP credentials is not found in configuration file");
        }
        _botClient = new TelegramBotClient(token);
    }

    public async Task SendMessageAsync(TelegramMessage message)
    {
        await _botClient.SendMessage(message.chatId, message.text);
    }
}