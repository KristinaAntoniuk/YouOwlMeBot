using YouOwlMeBot.Models;

namespace YouOwlMeBot.Services;

public interface IProfileService
{
    Task<(bool newProfile, Profile profile)> CreateProfile(string? profileName);
    Task<TgUserProfile> MapUserToTheProfile(Guid? userId, Guid? profileId);
    Task<string?> GetProfileNameById(Guid? profileId);
    Task<TgUserProfile?> GetUserProfileByUserId(Guid? userId);
    Task<IEnumerable<TgUserProfile>> GetUserProfileByProfileId(Guid? profileId);
}
