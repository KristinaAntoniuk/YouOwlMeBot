using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace YouOwlMeBot.Services
{
    public class UpdateService : IUpdateService
    {
        public Task HandleUpdate(Update update, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}
