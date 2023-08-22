using Amazon.DynamoDBv2.DataModel;
using YouOwlMeBot.Custom;
using YouOwlMeBot.DataProviders;
using YouOwlMeBot.Models;

namespace YouOwlMeBot.Services;

public class ProfileService : IProfileService
{
    private readonly ProfileDataProvider profileDataProvider;
    private readonly TgUserProfileDataProvider userProfileDataProvider;
    public ProfileService(IDynamoDBContext dbContext)
    {
        profileDataProvider = new ProfileDataProvider(dbContext);
        userProfileDataProvider = new TgUserProfileDataProvider(dbContext);
    }

    public async Task<(bool newProfile, Profile profile)> CreateProfile(string? profileName)
    {
        if (profileName == null) throw new Exception(Messages.ArgumentsCanNotBeEmpty);

        IEnumerable<Profile> profiles = await profileDataProvider.GetByName(profileName);

        if (!profiles.Any())
        {
            return (newProfile: true, profile: profileDataProvider.AddProfile(profileName).Result);
        }
        else return (newProfile: false, profile: profiles.First());
    }

    public async Task<string?> GetProfileNameById(Guid? profileId)
    {
        if (profileId == null) throw new Exception(Messages.ArgumentsCanNotBeEmpty);

        Profile? profile = await profileDataProvider.GetById(profileId.GetValueOrDefault());

        return profile?.Name;
    }

    public async Task<TgUserProfile?> GetUserProfileByUserId(Guid? userId)
    {
        if (userId == null) throw new Exception(Messages.SenderIsEmpty);

        IEnumerable<TgUserProfile> tgUserProfiles = await userProfileDataProvider.GetByUserId(userId.GetValueOrDefault());

        return tgUserProfiles.FirstOrDefault();
    }

    public async Task<TgUserProfile> MapUserToTheProfile(Guid? userId, Guid? profileId)
    {
        if (userId == null || profileId == null) throw new Exception(Messages.ArgumentsCanNotBeEmpty);

        return await userProfileDataProvider.AddUserProfile(userId.GetValueOrDefault(),
                                                            profileId.GetValueOrDefault());
    }

    public async Task<IEnumerable<TgUserProfile>> GetUserProfileByProfileId(Guid? profileId)
    {
        if (profileId == Guid.Empty) throw new Exception(Messages.ArgumentsCanNotBeEmpty);

        return await userProfileDataProvider.GetByProfileId(profileId.GetValueOrDefault());
    }
}
