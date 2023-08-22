using Telegram.Bot.Types;
using YouOwlMeBot.Models;

namespace YouOwlMeBot.Services;

public interface ITgUserService
{
    Task<Guid> GetUserId(User? user);
    Task<string?> GetFirstNameById(Guid? userId);
}
