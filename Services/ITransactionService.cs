using YouOwlMeBot.Models;

namespace YouOwlMeBot.Services;

public interface ITransactionService
{
    Task<Transaction> AddTransaction(Transaction transaction);
    Task<IEnumerable<Transaction>> GetAll(Guid? profileId);
    Task<IEnumerable<Transaction>> GetAll(Guid? profileId, Guid? userId);
    Task<IEnumerable<Transaction>> GetAllCurrentMonth(Guid? profileId);
    Task<IEnumerable<Transaction>> GetAllCurrentMonth(Guid? profileId, Guid? userId);
    Task<string?> GetBalances(Guid? profileId, bool currentMonth);
    Task<string?> GetLastPayments(Guid? profileId, int numberOfPayments);
}
