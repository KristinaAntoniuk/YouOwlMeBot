using Amazon.DynamoDBv2.DataModel;

namespace YouOwlMeBot.Models;

[DynamoDBTable("TgUserProfile")]
internal class TgUserProfile : IModel
{
    [DynamoDBHashKey("Id")]
    public Guid Id { get; set; }
    [DynamoDBProperty("UserId")]
    public Guid UserId { get; set; }
    [DynamoDBProperty("ProfileId")]
    public Guid ProfileId { get; set; }
}
