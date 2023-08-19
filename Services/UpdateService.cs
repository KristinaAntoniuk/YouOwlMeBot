using Telegram.Bot;
using Telegram.Bot.Types;
using YouOwlMeBot.Models;

namespace YouOwlMeBot.Services;
internal class UpdateService : IUpdateService
{
    private readonly ITelegramBotClient _botClient;
    private readonly ITgUserService _userService;
    private readonly IProfileService _profileService;

    private readonly ILogger<UpdateService> _logger;

    private static bool addingTransaction = false;
    private static bool addingProfile = false;
    private static bool addingRegistrationToken = false;
    private static Guid? userId = null;
    private static TgUserProfile? userProfile = null;
    private static Profile? profile = null;

    public UpdateService(ITelegramBotClient botClient,
                         ITgUserService userService,
                         IProfileService profileService,
                         ILogger<UpdateService> logger)
    {
        _botClient = botClient;
        _logger = logger;
        _userService = userService;
        _profileService = profileService;
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
                        userId ??= _userService.GetUserId(message.From).Result;
                        userProfile ??= _profileService.GetUserProfile(userId).Result;
                        await SendMessage(String.Format(Messages.Welcome, message.From?.FirstName));
                        break;
                    case "/addtransaction":
                        break;
                    case "/setnewprofile":
                        await SendMessage(Messages.YourProfileName);
                        addingProfile = true;
                        userId ??= _userService.GetUserId(message.From).Result;
                        userProfile ??= _profileService.GetUserProfile(userId).Result;
                        break;
                    case "/showprofile":
                        userId ??= _userService.GetUserId(message.From).Result;
                        userProfile ??= _profileService.GetUserProfile(userId).Result;

                        if (userProfile == null)
                        {
                            await SendMessage(Messages.YouAreNotMappedToAnyProfile);
                        }
                        else
                        {
                            string? profileName = _profileService.GetProfileNameById(userProfile.ProfileId).Result;
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

                            if (addingRegistrationToken)
                            {
                                string registrationToken = message.Text.Trim();
                                if (!String.Equals(profile?.RegistrationToken.ToString(), registrationToken))
                                {
                                    await SendMessage(Messages.WrongRegistrationToken);
                                    ResetFlags();
                                    break;
                                }
                            }
                            else
                            {
                                string profileName = message.Text.Trim();
                                (bool newProfile, profile) = _profileService.CreateProfile(profileName).Result;

                                if (newProfile)
                                {
                                    await SendMessage(String.Format(Messages.ProfileHasBeenCreated, profile.Name, profile.RegistrationToken));
                                    await SendMessage(message: profile.RegistrationToken.ToString());
                                }
                                else
                                {
                                    await SendMessage(String.Format(Messages.GetProfileRegistrationToken, profileName));
                                    addingRegistrationToken = true;
                                    break;
                                }

                                if (profile == null)
                                {
                                    await SendMessage(String.Format(Messages.ProfileHasNotBeenCreated, profileName));
                                }
                            }

                            userProfile = _profileService.MapUserToTheProfile(userId, profile?.Id).Result;

                            if (userProfile != null)
                            {
                                await SendMessage(String.Format(Messages.UserHasBeenAddedToTheProfile, profile?.Name));
                            }
                            else
                            {
                                await SendMessage(String.Format(Messages.UserHasNotBeenAddedToTheProfile, profile?.Name));
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
        addingRegistrationToken = false;
    }
}

