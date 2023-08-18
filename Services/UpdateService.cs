using Amazon.DynamoDBv2.DataModel;
using System;
using Telegram.Bot;
using Telegram.Bot.Types;
using YouOwlMeBot.DataProviders;
using YouOwlMeBot.Models;

namespace YouOwlMeBot.Services;

public interface IUpdateService
{
    Task HandleUpdate(Update update, CancellationToken cancellationToken = default);
}
internal class UpdateService : IUpdateService
{
    private readonly ITelegramBotClient _botClient;
    private readonly IDynamoDBContext _dbContext;

    private readonly ILogger<UpdateService> _logger;

    private static bool addingTransaction = false;
    private static bool addingProfile = false;
    private static Guid? userId = null;
    private static TgUserProfile? userProfile = null;

    public UpdateService(ITelegramBotClient botClient,
        IDynamoDBContext dbContext,
        ILogger<UpdateService> logger)
    {
        _botClient = botClient;
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task HandleUpdate(Update update, CancellationToken cancellationToken = default)
    {
        Message? message = update?.Message;

        try
        {
            if (message != null && message.Text != null && message.Text != String.Empty)
            {
                Chat chat = message.Chat;

                switch (message.Text)
                {
                    case "/start":
                        ResetFlags();
                        userId ??= GetUserId(message.From);
                        userProfile ??= GetUserProfile(userId);
                        await SendMessage(String.Format(Messages.Welcome, message.From?.FirstName));
                        break;
                    case "/addtransaction":
                        break;
                    case "/setnewprofile":
                        await SendMessage(Messages.YourProfileName);
                        addingProfile = true;
                        userId ??= GetUserId(message.From);
                        userProfile ??= GetUserProfile(userId);
                        break;
                    case "/showprofile":
                        userId ??= GetUserId(message.From);
                        userProfile ??= GetUserProfile(userId);

                        if (userProfile == null)
                        {
                            await SendMessage(Messages.YouAreNotMappedToAnyProfile);
                        }
                        else
                        {
                            string? profileName = GetProfileNameById(userProfile.ProfileId);
                            if (String.IsNullOrEmpty(profileName))
                            {
                                await SendMessage(Messages.ProfileDoesNotExist);
                            }

                            await SendMessage(String.Format(Messages.CurrentProfile, profileName));
                        }
                        break;
                    case "/showbalance":
                        break;
                    default:
                        if (addingTransaction)
                        {
                        }
                        if (addingProfile)
                        {
                            string profileName = message.Text.Trim();
                            (bool newProfile, Guid? profileId) = CreateProfile(profileName);
                            userProfile = MapUserToTheProfile(userId, profileId);

                            if (newProfile)
                            {
                                await SendMessage(String.Format(Messages.ProfileHasBeenCreated, profileName));
                            }

                            if (profileId == null)
                            {
                                await SendMessage(String.Format(Messages.ProfileHasNotBeenCreated, profileName));
                            }

                            if (userProfile != null)
                            {
                                await SendMessage(String.Format(Messages.UserHasBeenAddedToTheProfile, profileName));
                            }
                            else
                            {
                                await SendMessage(String.Format(Messages.UserHasNotBeenAddedToTheProfile, profileName));
                            }
                            ResetFlags();
                        }
                        break;
                }

                async Task SendMessage(string message)
                {
                    await _botClient.SendTextMessageAsync(chat.Id, message, cancellationToken: cancellationToken);
                }

            }
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "Unknown error", new
            {
                Message = message
            });
        }
    }

    private void ResetFlags()
    {
        addingTransaction = false;
        addingProfile = false;
        userId = null;
        userProfile = null;
    }

    private Guid? GetUserId(User? tgUser)
    {
        if (tgUser == null) throw new Exception(Messages.SenderIsEmpty);
        if (String.IsNullOrEmpty(tgUser.Username)) throw new Exception(Messages.UserNameCanNotBeEmpty);

        TgUserDataProvider userDataProvider = new TgUserDataProvider(_dbContext);
        
        userId = userDataProvider.GetByUsername(tgUser.Username).Result?.FirstOrDefault()?.Id;

        return userId ?? userDataProvider.AddUser(tgUser).Result;
        
    }

    private TgUserProfile? GetUserProfile(Guid? userId)
    {
        if (userId == null) throw new Exception(Messages.SenderIsEmpty);

        TgUserProfileDataProvider userProfileDataProvider = new TgUserProfileDataProvider(_dbContext);
        
        return userProfileDataProvider.GetByUserId(userId.GetValueOrDefault()).Result?.FirstOrDefault();
    }

    private string? GetProfileNameById(Guid? profileId)
    {
        if (profileId == null) throw new Exception(Messages.ArgumentsCanNotBeEmpty);

        ProfileDataProvider profileDataProvider = new ProfileDataProvider(_dbContext);

        return profileDataProvider.GetById(profileId.GetValueOrDefault()).Result?.Name;
    }

    private (bool newProfile, Guid? profileId) CreateProfile(string? profileName)
    {
        if (profileName == null) throw new Exception(Messages.ArgumentsCanNotBeEmpty);

        ProfileDataProvider profileDataProvider = new ProfileDataProvider(_dbContext);
        Guid? profileId = profileDataProvider.GetByName(profileName).Result?.FirstOrDefault()?.Id;

        if (profileId == null)
        {
            profileId = profileDataProvider.AddProfile(profileName).Result;
            return (newProfile: true, profileId: profileId);
        }
        else return (newProfile: false, profileId: profileId);
    }

    private TgUserProfile MapUserToTheProfile(Guid? userId, Guid? profileId)
    {
        if (userId == null || profileId == null) throw new Exception(Messages.ArgumentsCanNotBeEmpty);

        TgUserProfileDataProvider userProfileDataProvider = new TgUserProfileDataProvider(_dbContext);
        return userProfileDataProvider.AddUserProfile(userId.GetValueOrDefault(),
                                                      profileId.GetValueOrDefault()).Result;
    }
}
