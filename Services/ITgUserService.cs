using Telegram.Bot.Types;
using YouOwlMeBot.Models;

namespace YouOwlMeBot.Services;

public interface ITgUserService
{
    Task<TgUser> GetTgUser(User? user);
    Task<string?> GetFirstNameById(Guid? userId);
}
