using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using YouOwlMeBot.Custom;
using YouOwlMeBot.Models;

namespace YouOwlMeBot.DataProviders;

internal class TgUserProfileDataProvider : ContextProvider, IDataProvider<TgUserProfile>
{
    public TgUserProfileDataProvider(IDynamoDBContext dynamoDBContext) : base(dynamoDBContext) { }

    public async void Delete(TgUserProfile data)
    {
        await _dynamoDBContext.DeleteAsync(data);
    }

    public async Task<IEnumerable<TgUserProfile>> GetAll()
    {
        return await _dynamoDBContext.ScanAsync<TgUserProfile>(default).GetRemainingAsync();
    }

    public async Task<TgUserProfile> GetById(Guid? guid)
    {
        return await _dynamoDBContext.LoadAsync<TgUserProfile>(guid);
    }

    public async Task Save(TgUserProfile data)
    {
        await _dynamoDBContext.SaveAsync(data);
    }

    internal async Task<IEnumerable<TgUserProfile>> GetByUserId(Guid userId)
    {
        if (userId == Guid.Empty) throw new Exception(Messages.ArgumentsCanNotBeEmpty);

        IEnumerable<ScanCondition> conditions = new List<ScanCondition>
        {
            new ScanCondition("UserId", ScanOperator.Equal, userId)
        };

        return await _dynamoDBContext.ScanAsync<TgUserProfile>(conditions).GetRemainingAsync();
    }

    internal async Task<IEnumerable<TgUserProfile>> GetByProfileId(Guid profileId)
    {
        if (profileId == Guid.Empty) throw new Exception(Messages.ArgumentsCanNotBeEmpty);

        IEnumerable<ScanCondition> conditions = new List<ScanCondition>
        {
            new ScanCondition("ProfileId", ScanOperator.Equal, profileId)
        };

        return await _dynamoDBContext.ScanAsync<TgUserProfile>(conditions).GetRemainingAsync();
    }

    internal async Task<IEnumerable<TgUserProfile>> GetByUserIdAndProfileId(Guid userId, Guid profileId)
    {
        if (userId == Guid.Empty || profileId == Guid.Empty) throw new Exception(Messages.ArgumentsCanNotBeEmpty);

        IEnumerable<ScanCondition> conditions = new List<ScanCondition>
        {
            new ScanCondition("UserId", ScanOperator.Equal, userId),
            new ScanCondition("ProfileId", ScanOperator.Equal, profileId)
        };

        return await _dynamoDBContext.ScanAsync<TgUserProfile>(conditions).GetRemainingAsync();
    }
    internal async Task<TgUserProfile> AddUserProfile(Guid userID, Guid profileId)
    {
        if (userID == Guid.Empty || profileId == Guid.Empty) throw new Exception(Messages.ArgumentsCanNotBeEmpty);

        TgUserProfile? existingUserProfile = GetByUserId(userID).Result?.FirstOrDefault();

        if (existingUserProfile != null)
        {
            Delete(existingUserProfile);
        }

        TgUserProfile profile = new TgUserProfile()
        {
            Id = Guid.NewGuid(),
            UserId = userID,
            ProfileId = profileId
        };

        await Save(profile);

        return profile;
    }
}
