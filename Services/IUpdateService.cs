using Telegram.Bot.Types;

namespace YouOwlMeBot.Services;

public interface IUpdateService
{
    Task HandleUpdate(Update update, CancellationToken cancellationToken = default);
}
