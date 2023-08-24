using Amazon.DynamoDBv2.DataModel;

namespace YouOwlMeBot.Models;

[DynamoDBTable("Profile")]
public class Profile : IModel
{
    [DynamoDBHashKey("Id")]
    public Guid Id { get; set; }
    [DynamoDBProperty("Name")]
    public string? Name { get; set; }
    [DynamoDBProperty("RegistrationToken")]
    public Guid RegistrationToken { get; set; }
}
