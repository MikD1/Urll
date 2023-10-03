using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Urll.Links.Contracts;

namespace Urll.TelegramBot;

public class Worker : IHostedService
{
    public Worker(ILogger<Worker> logger, IConfiguration configuration, ILinksClient linksClient)
    {
        _logger = logger;
        _configuration = configuration;
        _linksClient = linksClient;
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

        CommandsExecutor executor = new(_linksClient);
        string result = await executor.Execute(messageText);
        string userId = update.Message.Chat.Id.ToString();
        await SendMessage(userId, result);

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
    private readonly ILinksClient _linksClient;
    private ITelegramBotClient _botClient = default!;
    private CancellationTokenSource _receivingCts = default!;
}
