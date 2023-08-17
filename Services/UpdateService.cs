using Amazon.DynamoDBv2.DataModel;
using Telegram.Bot;
using Telegram.Bot.Types;
using YouOwlMeBot.DataProviders;

namespace YouOwlMeBot.Services;

public interface IUpdateService
{
    Task HandleUpdate(Update update, IDynamoDBContext context, CancellationToken cancellationToken = default);
}
internal class UpdateService : IUpdateService
{
    private readonly ITelegramBotClient _botClient;

    private readonly ILogger<UpdateService> _logger;

    static bool addingTransaction = false;
    static bool addingProfile = false;
    static string? storeName = null;
    static decimal amount = 0;
    static string? username;

    public UpdateService(ITelegramBotClient botClient,
        ILogger<UpdateService> logger)
    {
        _botClient = botClient;
        _logger = logger;
    }

    public async Task HandleUpdate(Update update, IDynamoDBContext context, CancellationToken cancellationToken = default)
    {
        Message? message = update?.Message;

        try
        {
            if (message != null)
            {
                switch (message.Text)
                {
                    case "/start":
                        await _botClient.SendTextMessageAsync(update.Message.Chat.Id,
                                                              Messages.YourProfileName,
                                                              cancellationToken: cancellationToken);
                        username = message.Chat.Username;
                        addingTransaction = false;
                        addingProfile = true;
                        storeName = null;
                        amount = 0;
                        break;
                    case "/add":
                        break;
                    default:
                        if (addingTransaction)
                        {
                        }
                        if (addingProfile)
                        {
                            Guid? userID = null;
                            using (UserDataProvider userDataProvider = new UserDataProvider(context))
                            {
                                userID = userDataProvider.GetUserByUsername(username).Result?.FirstOrDefault()?.Guid;
                                if (userID == null)
                                {
                                    userDataProvider.AddUser(message.From);
                                    userID = userDataProvider.GetUserByUsername(username).Result?.FirstOrDefault()?.Guid;
                                }
                                else
                                {

                                }
                            }
                        }
                        break;
                }
            }
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "Unknown error", new
            {
                Message = message
            });
        }
    }
}
