using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using YouOwlMeBot.Models;

namespace YouOwlMeBot.DataProviders;

internal class ProfileDataProvider : ContextProvider, IDataProvider<Profile>
{
    public ProfileDataProvider(IDynamoDBContext dynamoDBContext) : base(dynamoDBContext) { }

    public async void Delete(Profile data)
    {
        await _dynamoDBContext.DeleteAsync(data);
    }

    public async Task<IEnumerable<Profile>> GetAll()
    {
        return await _dynamoDBContext.ScanAsync<Profile>(default).GetRemainingAsync();
    }

    public async Task<Profile> GetById(Guid? guid)
    {
        return await _dynamoDBContext.LoadAsync<Profile>(guid);
    }

    public async Task Save(Profile data)
    {
        await _dynamoDBContext.SaveAsync(data);
    }

    internal async Task<IEnumerable<Profile>> GetByName(string name)
    {
        IEnumerable<ScanCondition> conditions = new List<ScanCondition>
        {
            new ScanCondition("Name", ScanOperator.Equal, name)
        };

        return await _dynamoDBContext.ScanAsync<Profile>(conditions).GetRemainingAsync();
    }

    internal async Task<Profile> AddProfile(string name)
    {
        Profile profile = new Profile()
        {
            Id = Guid.NewGuid(),
            Name = name,
            RegistrationToken = Guid.NewGuid()
        };

        await Save(profile);
        return profile;
    }
}
