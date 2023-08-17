using Amazon.DynamoDBv2.DataModel;

namespace YouOwlMeBot.Models;

[DynamoDBTable("Transaction")]
internal class Transaction : IModel
{
    [DynamoDBHashKey("Id")]
    public int Id { get; set; }
    [DynamoDBProperty("UserID")]
    public int? UserID { get; set; }
    [DynamoDBProperty("Date")]
    public required DateTime Date { get; set; }
    [DynamoDBProperty("Store")]
    public string? Store { get; set; }
    [DynamoDBProperty("Amount")]
    public required decimal Amount { get; set; }
    [DynamoDBProperty("Guid")]
    public required Guid Guid { get; set; }
}
