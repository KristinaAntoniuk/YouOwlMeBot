using YouOwlMeBot.Models;

namespace YouOwlMeBot.Services;

public interface ITransactionService
{
    Task<Transaction> AddTransaction(Transaction transaction);
    Task<IEnumerable<Transaction>> GetAll(Guid? profileId);
    Task<IEnumerable<Transaction>> GetAll(Guid? profileId, Guid? userId);
    Task<IEnumerable<Transaction>> GetAllCurrentMonth(Guid? profileId);
    Task<decimal> GetTotalCurrentMonth(Guid? profileId);
    Task<decimal> GetTotalPreviousMonth(Guid? profileId);
    Task<IEnumerable<Transaction>> GetAllPreviousMonth(Guid? profileId);
    Task<IEnumerable<Transaction>> GetAllCurrentMonth(Guid? profileId, Guid? userId);
    Task<string?> GetBalances(Guid? profileId);
    Task<string?> GetCurrentPayments(Guid? profileId);
    Task<string?> GetPreviousPayments(Guid? profileId);
}
