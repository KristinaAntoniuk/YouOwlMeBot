namespace YouOwlMeBot;

public static class Messages
{
    public static string RecordHasBeenSaved = "Record {0} - {1} has been saved.";
    public static string RecordHasNotBeenSaved = "Record {0} - {1} has NOT been saved. Try again.";
    public static string UserNameCanNotBeEmpty = "Username can not be empty.";
    public static string SenderIsEmpty = "Sender is empty.";
    public static string ArgumentsCanNotBeEmpty = "Argument can not be empty";
    public static string Welcome = "Welcome, {0}!";
    public static string CommendIsNotRecognized = "Command is not recognized";

    #region Profiles
    public static string YourProfileName = "Profile Name:";
    public static string ProfileHasBeenCreated = "The new profile - {0} has been created. Use this registration token to add new users:";
    public static string ProfileHasNotBeenCreated = "The new profile - {0} has NOT been created.";
    public static string UserHasBeenAddedToTheProfile = "Your were successfully added to the profile - {0}.";
    public static string UserHasNotBeenAddedToTheProfile = "Your were NOT added to the profile - {0}.";
    public static string CurrentProfile = "Your current profile - {0}.";
    public static string YouAreNotMappedToAnyProfile = "You aren't mapped to any profile yet.";
    public static string ProfileDoesNotExist = "Profile doesn't exist.";
    public static string GetProfileRegistrationToken = "Profile - {0} already exists. Registration token:";
    public static string WrongRegistrationToken = "Registration token is wrong. Try once more or consider creating the new profile.";
    #endregion
}
