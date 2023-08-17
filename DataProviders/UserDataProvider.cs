using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using YouOwlMeBot.Models;

namespace YouOwlMeBot.DataProviders
{
    internal class UserDataProvider : IDataProvider<User>, IDisposable
    {
        public IDynamoDBContext _dynamoDBContext { get; set; }

        internal UserDataProvider(IDynamoDBContext dynamoDBContext) 
        {
            _dynamoDBContext = dynamoDBContext;
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _dynamoDBContext.ScanAsync<User>(default).GetRemainingAsync();
        }

        public async Task<User> GetByGuid(Guid? guid)
        {
            return await _dynamoDBContext.LoadAsync<User>(guid);
        }

        public async void Delete(User data)
        {
            await _dynamoDBContext.DeleteAsync(data);
        }

        public async Task Save(User data)
        {
            await _dynamoDBContext.SaveAsync(data);
        }

        public void Dispose()
        {
            _dynamoDBContext.Dispose();
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

            await Save(user);
        }
    }
}
