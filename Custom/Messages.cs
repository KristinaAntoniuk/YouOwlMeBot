namespace YouOwlMeBot.Custom;

public static class Messages
{
    public static string PaymentHasBeenSaved(string store, decimal? amount) => $"Payment {store} - {amount} has been saved.";
    public static string RecordHasNotBeenSaved(string store, decimal? amount) => $"Record {store} - {amount} has NOT been saved. Try again.";
    public static string UserNameCanNotBeEmpty => $"Username can not be empty.";
    public static string SenderIsEmpty => $"Sender is empty.";
    public static string ArgumentsCanNotBeEmpty = $"Argument can not be empty";
    public static string Welcome(string? name) => $"Welcome, {name}!";
    public static string CommendIsNotRecognized = $"Command is not recognized";
    public static string StoreName => $"Store Name:";
    public static string Amount => $"Amount:";
    public static string RepaymentHasBeenSaved => $"Repayment has been saved.";
    public static string NoTransactions => $"There are no transactions for the selected period";
    public static string ThereIsOnlyOneUserInTheProfile => $"There is only one user mapped to the profile. Try to add at least one more to be able to count balances.";
    public static string CommandIsNotRecognized => $"Sorry, I can't recognize this command.";
    public static string NumberIsIncorrect => $"Number is incorrect";
    public static string BotIsNotAvailable => $"Sorry, but this bot is private and is available for the restricted amount of users.";
    #region Profiles
    public static string YourProfileName => $"Profile Name:";
    public static string ProfileHasBeenCreated(string? profileName) => $"The new profile - {profileName} has been created. Use this registration token to add new users:";
    public static string ProfileHasNotBeenCreated(string? profileName) => $"The new profile - {profileName} has NOT been created.";
    public static string UserHasBeenAddedToTheProfile(string? profileName) => $"Your were successfully added to the profile - {profileName}.";
    public static string UserHasNotBeenAddedToTheProfile(string? profileName) => $"Your were NOT added to the profile - {profileName}.";
    public static string CurrentProfile(string? profileName) => $"Your current profile - {profileName}.";
    public static string YouAreNotMappedToAnyProfile => $"You aren't mapped to any profile yet.";
    public static string ProfileDoesNotExist => $"Profile doesn't exist.";
    public static string GetProfileRegistrationToken(string? profileName) => $"Profile - {profileName} already exists. Registration token:";
    public static string WrongRegistrationToken => $"Registration token is wrong. Try once more or consider creating the new profile.";
    #endregion
}
