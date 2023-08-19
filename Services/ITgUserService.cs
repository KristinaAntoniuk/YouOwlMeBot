using Telegram.Bot.Types;

namespace YouOwlMeBot.Services
{
    public interface ITgUserService
    {
        Task<Guid?> GetUserId(User? user);
    }
}
