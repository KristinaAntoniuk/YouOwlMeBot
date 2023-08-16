namespace misha_kris_finance_bot.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int? UserID { get; set; }
        public required DateTime Date { get; set; }
        public string? Store { get; set; }
        public required decimal Amount { get; set; }
        public required Guid Guid { get; set; }
    }
}
