using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using YouOwlMeBot.Models;

namespace YouOwlMeBot.DataProviders
{
    internal class UserDataProvider : IDisposable
    {
        private readonly IDynamoDBContext _dynamoDBContext;
        internal UserDataProvider(IDynamoDBContext dynamoDBContext) 
        {
            _dynamoDBContext = dynamoDBContext;
        }

        internal async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _dynamoDBContext.ScanAsync<User>(default).GetRemainingAsync();
        }

        internal async Task<User> GetUserById(int id)
        {
            return await _dynamoDBContext.LoadAsync<User>(id);
        }

        internal async Task<IEnumerable<User>> GetUserByUsername(string username)
        {
            IEnumerable<ScanCondition> conditions = new List<ScanCondition>
            {
                new ScanCondition("Username", ScanOperator.Equal, username)
            };

            return await _dynamoDBContext.ScanAsync<User>(conditions).GetRemainingAsync();
        }

        internal async void AddUser(Telegram.Bot.Types.User tgUser)
        {
            User user = new User()
            {
                Guid = Guid.NewGuid(),
                Username = tgUser.Username,
                LastName = tgUser.LastName,
                FirstName = tgUser.FirstName
            };

            await SaveUser(user);
        }

        internal async void DeleteUser(User user)
        {
            await _dynamoDBContext.DeleteAsync(user);
        }

        internal async void UpdateUser(User user)
        {
            await SaveUser(user);
        }

        private async Task SaveUser(User user)
        {
            await _dynamoDBContext.SaveAsync(user);
        }

        public void Dispose()
        {
            _dynamoDBContext.Dispose();
        }
    }
}
