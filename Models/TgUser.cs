using Amazon.DynamoDBv2.DataModel;

namespace YouOwlMeBot.Models;

[DynamoDBTable("TgUser")]
internal class TgUser : IModel
{
    [DynamoDBHashKey("Id")]
    public Guid Id { get; set; }
    [DynamoDBProperty("Username")]
    public string? Username { get; set; }
    [DynamoDBProperty("FirstName")]
    public string? FirstName { get; set; }
    [DynamoDBProperty("LastName")]
    public string? LastName { get; set;}
}
