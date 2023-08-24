using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using YouOwlMeBot.Models;

namespace YouOwlMeBot.DataProviders;

internal class TgUserDataProvider : ContextProvider, IDataProvider<TgUser>
{
    public TgUserDataProvider(IDynamoDBContext dynamoDBContext) : base(dynamoDBContext) { }

    public async Task<IEnumerable<TgUser>> GetAll()
    {
        return await _dynamoDBContext.ScanAsync<TgUser>(default).GetRemainingAsync();
    }

    public async Task<TgUser> GetById(Guid? guid)
    {
        return await _dynamoDBContext.LoadAsync<TgUser>(guid);
    }

    public async void Delete(TgUser data)
    {
        await _dynamoDBContext.DeleteAsync(data);
    }

    public async Task Save(TgUser data)
    {
        await _dynamoDBContext.SaveAsync(data);
    }
    internal async Task<IEnumerable<TgUser>> GetByUsername(string username)
    {
        IEnumerable<ScanCondition> conditions = new List<ScanCondition>
        {
            new ScanCondition("Username", ScanOperator.Equal, username)
        };

        return await _dynamoDBContext.ScanAsync<TgUser>(conditions).GetRemainingAsync();
    }

    internal async Task<TgUser> AddUser(Telegram.Bot.Types.User tgUser)
    {
        TgUser user = new()
        {
            Id = Guid.NewGuid(),
            Username = tgUser.Username,
            LastName = tgUser.LastName,
            FirstName = tgUser.FirstName
        };

        await Save(user);

        return user;
    }
}
