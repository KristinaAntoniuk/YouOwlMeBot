using Amazon.DynamoDBv2.DataModel;
using YouOwlMeBot.Custom;

namespace YouOwlMeBot.Models;

[DynamoDBTable("Transaction")]
public class Transaction : IModel
{
    [DynamoDBHashKey("Id")]
    public Guid Id { get; set; }
    [DynamoDBProperty("UserId")]
    public Guid? UserId { get; set; }
    [DynamoDBProperty("Date")]
    public required DateTime Date { get; set; }
    [DynamoDBProperty("Store")]
    public string? Store { get; set; }
    [DynamoDBProperty("Amount")]
    public required decimal Amount { get; set; }
    [DynamoDBProperty("ProfileId")]
    public Guid? ProfileId { get; set; }
    [DynamoDBProperty("Type")]
    public int? Type { get; set; }

    public string ToString(string? firstName)
    {
        return String.Format("{0} - {1} {2} {3} {4}", Date.ToShortDateString(), Store, Amount.ToString(), 
                                                      Enum.GetName(typeof(TransactionType), Type.GetValueOrDefault()), firstName);
    }
}
