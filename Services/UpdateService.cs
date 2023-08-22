using Telegram.Bot;
using Telegram.Bot.Types;
using YouOwlMeBot.Custom;
using YouOwlMeBot.Models;

namespace YouOwlMeBot.Services;
internal class UpdateService : IUpdateService
{
    private readonly ITelegramBotClient _botClient;
    private readonly ITgUserService _tgUserService;
    private readonly IProfileService _profileService;
    private readonly ITransactionService _transactionService;

    private readonly ILogger<UpdateService> _logger;

    private static bool addingPayment = false;
    private static bool addingRepayment = false;
    private static bool addingProfile = false;
    private static bool addingRegistrationToken = false;
    private static string? storeName;
    private static Guid? userId = null;
    private static TgUserProfile? userProfile = null;
    private static Profile? profile = null;

    public UpdateService(ITelegramBotClient botClient,
                         ITgUserService tgUserService,
                         IProfileService profileService,
                         ITransactionService transactionService,
                         ILogger<UpdateService> logger)
    {
        _botClient = botClient;
        _logger = logger;
        _tgUserService = tgUserService;
        _profileService = profileService;
        _transactionService = transactionService;
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
                        userId ??= _tgUserService.GetUserId(message.From).Result;
                        userProfile ??= _profileService.GetUserProfileByUserId(userId).Result;
                        await SendMessage(String.Format(Messages.Welcome, message.From?.FirstName));
                        break;
                    case "/addpayment":
                        ResetFlags();
                        userId ??= _tgUserService.GetUserId(message.From).Result;
                        userProfile ??= _profileService.GetUserProfileByUserId(userId).Result;
                        if (userProfile == null)
                        {
                            await SendMessage(Messages.YouAreNotMappedToAnyProfile);
                        }
                        else
                        {
                            await SendMessage(Messages.StoreName);
                            addingPayment = true;
                        }
                        break;
                    case "/addrepayment":
                        ResetFlags();
                        userId ??= _tgUserService.GetUserId(message.From).Result;
                        userProfile ??= _profileService.GetUserProfileByUserId(userId).Result;
                        if (userProfile == null)
                        {
                            await SendMessage(Messages.YouAreNotMappedToAnyProfile);
                        }
                        else
                        {
                            await SendMessage(Messages.Amount);
                            addingRepayment = true;
                        }
                        break;
                    case "/setnewprofile":
                        ResetFlags();
                        userProfile = null;
                        await SendMessage(Messages.YourProfileName);
                        addingProfile = true;
                        userId ??= _tgUserService.GetUserId(message.From).Result;
                        break;
                    case "/showprofile":
                        ResetFlags();
                        userId ??= _tgUserService.GetUserId(message.From).Result;
                        userProfile ??= _profileService.GetUserProfileByUserId(userId).Result;

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
                        ResetFlags();
                        userId ??= _tgUserService.GetUserId(message.From).Result;
                        userProfile ??= _profileService.GetUserProfileByUserId(userId).Result;
                        string? allBalances = await _transactionService.GetBalances(userProfile?.ProfileId, false);
                        if (!String.IsNullOrEmpty(allBalances)) await SendMessage(allBalances);
                        break;
                    case "/showlastbalance":
                        ResetFlags();
                        userId ??= _tgUserService.GetUserId(message.From).Result;
                        userProfile ??= _profileService.GetUserProfileByUserId(userId).Result;
                        string? lastBalances = await _transactionService.GetBalances(userProfile?.ProfileId, true);
                        if (!String.IsNullOrEmpty(lastBalances)) await SendMessage(lastBalances);
                        break;
                    case "/showlasttransactions":
                        ResetFlags();
                        userId ??= _tgUserService.GetUserId(message.From).Result;
                        userProfile ??= _profileService.GetUserProfileByUserId(userId).Result;

                        string? lastPayments = await _transactionService.GetLastPayments(userProfile?.ProfileId, 5);

                        if (lastPayments != null)
                        {

                            await SendMessage(lastPayments);
                        }
                        else
                        {
                            await SendMessage(Messages.NoTransactions);
                        }
                        break;
                    default:
                        if (addingPayment)
                        {
                            if (storeName == null)
                            {
                                storeName = message.Text.Trim();
                                await SendMessage(Messages.Amount);
                            }
                            else
                            {
                                decimal amount;
                                if (Decimal.TryParse(message.Text.Trim(), out amount))
                                {
                                    Transaction transaction = new Transaction()
                                    {
                                        Id = Guid.NewGuid(),
                                        Amount = amount,
                                        Date = DateTime.UtcNow,
                                        ProfileId = userProfile?.ProfileId,
                                        Store = storeName,
                                        Type = (int)TransactionType.Payment,
                                        UserId = userId
                                    };

                                    bool successTran = _transactionService.AddTransaction(transaction) != null;
                                    if (successTran)
                                    {
                                        await SendMessage(String.Format(Messages.PaymentHasBeenSaved, storeName, amount.ToString()));
                                        ResetFlags();
                                    }
                                }
                            }
                            
                        }
                        else if (addingRepayment)
                        {
                            decimal amount;
                            if (Decimal.TryParse(message.Text.Trim(), out amount))
                            {
                                Transaction transaction = new Transaction()
                                {
                                    Id = Guid.NewGuid(),
                                    Amount = amount,
                                    Date = DateTime.UtcNow,
                                    ProfileId = userProfile?.ProfileId,
                                    Store = null,
                                    Type = (int)TransactionType.Repayment,
                                    UserId = userId
                                };

                                bool successTran = _transactionService.AddTransaction(transaction) != null;
                                if (successTran)
                                {
                                    await SendMessage(Messages.RepaymentHasBeenSaved);
                                    ResetFlags();
                                }
                            }
                        }
                        else if (addingProfile)
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
        addingPayment = false;
        addingRepayment = false;
        addingProfile = false;
        addingRegistrationToken = false;
        storeName = null;
    }
}

