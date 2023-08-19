using Amazon.DynamoDBv2.DataModel;
using Telegram.Bot.Types;
using YouOwlMeBot.DataProviders;
using YouOwlMeBot.Models;

namespace YouOwlMeBot.Services
{
    public class TgUserService : ITgUserService
    {
        private readonly TgUserDataProvider userDataProvider;
        public TgUserService(IDynamoDBContext dbContext)
        {
            userDataProvider = new TgUserDataProvider(dbContext);
        }
        public async Task<Guid?> GetUserId(User? user)
        {
            if (user == null) throw new Exception(Messages.SenderIsEmpty);
            if (String.IsNullOrEmpty(user.Username)) throw new Exception(Messages.UserNameCanNotBeEmpty);

            IEnumerable<TgUser> users = await userDataProvider.GetByUsername(user.Username);

            return users?.FirstOrDefault()?.Id ?? userDataProvider.AddUser(user).Result;
        }
    }
}
