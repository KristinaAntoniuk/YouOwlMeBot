using Amazon.DynamoDBv2.DataModel;

namespace YouOwlMeBot.Models;

[DynamoDBTable("User")]
internal class User : IModel
{
    [DynamoDBHashKey("Guid")]
    public Guid Guid { get; set; }
    [DynamoDBProperty("Username")]
    public string? Username { get; set; }
    [DynamoDBProperty("FirstName")]
    public string? FirstName { get; set; }
    [DynamoDBProperty("LastName")]
    public string? LastName { get; set;}
}
