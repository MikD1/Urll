using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Urll.TelegramBot;

public class Worker : IHostedService
{
    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        string token = _configuration["TelegramBotToken"]!;
        _botClient = new TelegramBotClient(token);
        _receivingCts = new CancellationTokenSource();

        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        _botClient.StartReceiving(HandleUpdate, HandleError, receiverOptions, _receivingCts.Token);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _receivingCts.Cancel();
        return Task.CompletedTask;
    }

    public async Task SendMessage(string userId, string message)
    {
        await _botClient.SendTextMessageAsync(userId, message, replyMarkup: new ReplyKeyboardRemove());
        _logger.LogInformation($"Message '{message}' send to user '{userId}'");
    }

    private async Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Message is null)
        {
            return;
        }

        string? messageText = update.Message.Text;
        if (messageText is null)
        {
            return;
        }

        string userId = update.Message.Chat.Id.ToString();

        if (string.Equals(messageText, "/start", StringComparison.OrdinalIgnoreCase))
        {
            await SendMessage(userId, "Hello! This is Urll bot");
        }
        else if (string.Equals(messageText, "/list", StringComparison.OrdinalIgnoreCase))
        {
            await SendMessage(userId, "List all Links");
        }
        else if (messageText.StartsWith("/get", StringComparison.OrdinalIgnoreCase))
        {
            string[] parts = messageText.Split(' ');
            if (parts.Length is 2)
            {
                await SendMessage(userId, $"Get Link with code '{parts[1]}'");
            }
        }
        else if (messageText.StartsWith("/delete", StringComparison.OrdinalIgnoreCase))
        {
            string[] parts = messageText.Split(' ');
            if (parts.Length is 2)
            {
                await SendMessage(userId, $"Delete Link with code '{parts[1]}'");
            }
        }
        else
        {
            string[] parts = messageText.Split(' ');
            if (parts.Length is 2)
            {
                await SendMessage(userId, $"Add Link\nUrl: ${parts[0]}\nCode '{parts[1]}'");
            }
        }

        _logger.LogInformation($"HandleTextUpdate: '{userId}' : '{messageText}'");
    }

    private Task HandleError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        string errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        _logger.LogError(errorMessage);
        return Task.CompletedTask;
    }

    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;
    private ITelegramBotClient _botClient = default!;
    private CancellationTokenSource _receivingCts = default!;
}
