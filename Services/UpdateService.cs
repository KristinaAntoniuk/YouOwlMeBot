using Amazon.DynamoDBv2.DataModel;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace misha_kris_finance_lambda_bot.Services
{
    public interface IUpdateService
    {
        Task HandleUpdate(Update update, IDynamoDBContext context, CancellationToken cancellationToken = default);
    }
    internal class UpdateService : IUpdateService
    {
        private const string WelcomeMessage = "Hello Misha!";
        
        private readonly ITelegramBotClient _botClient;

        private readonly ILogger<UpdateService> _logger;

        public UpdateService(ITelegramBotClient botClient,
            ILogger<UpdateService> logger)
        {
            _botClient = botClient;
            _logger = logger;
        }

        public async Task HandleUpdate(Update update, IDynamoDBContext context, CancellationToken cancellationToken = default)
        {
            var message = update?.Message?.Text;
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            try
            {
                //if (message.Equals("/start") == true)
                //{
                //    await _botClient.SendTextMessageAsync(update.Message.Chat.Id,
                //        WelcomeMessage,
                //        cancellationToken: cancellationToken);

                //    return;
                //}

                try
                {
                    //var response = await _translatorClient.Translate(message, cancellationToken);
                    //var translated = response?.Contents?.Translated;

                    //if (string.IsNullOrEmpty(translated))
                    //{
                    //    throw new Exception("Translated content is null or empty");
                    //}

                    List<Models.User>? users = await context.ScanAsync<Models.User>(default).GetRemainingAsync();

                    foreach(Models.User user in  users)
                    {
                        await _botClient.SendTextMessageAsync(update.Message.Chat.Id,
                        user.Username,
                        replyToMessageId: update.Message.MessageId,
                        cancellationToken: cancellationToken);
                    }
                    
                }
                catch (Exception ex)
                {
                    await _botClient.SendTextMessageAsync(update.Message.Chat.Id,
                        ex.Message,
                        replyToMessageId: update.Message.MessageId,
                        cancellationToken: cancellationToken);

                    throw;
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
}
