using Amazon.DynamoDBv2.DataModel;

namespace misha_kris_finance_lambda_bot.Models;
[DynamoDBTable("User")]
internal class User
{
    [DynamoDBHashKey("Id")]
    public int Id { get; set; }
    [DynamoDBProperty("Username")]
    public string? Username { get; set; }
}
