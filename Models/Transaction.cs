using Amazon.DynamoDBv2.DataModel;

namespace misha_kris_finance_lambda_bot.Models;

[DynamoDBTable("Transaction")]
internal class Transaction
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
