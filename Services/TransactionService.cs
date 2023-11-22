using Amazon.DynamoDBv2.DataModel;
using System.Text;
using YouOwlMeBot.Custom;
using YouOwlMeBot.DataProviders;
using YouOwlMeBot.Models;

namespace YouOwlMeBot.Services;

public class TransactionService : ITransactionService
{
    private readonly ITgUserService _tgUserService;
    private readonly IProfileService _profileService;
    private readonly TransactionDataProvider transactionDataProvider;
    public TransactionService(IDynamoDBContext dbContext, ITgUserService tgUserService, IProfileService profileService)
    {
        transactionDataProvider = new TransactionDataProvider(dbContext);
        _tgUserService = tgUserService;
        _profileService = profileService;
    }
    public async Task<Transaction> AddTransaction(Transaction transaction)
    {
        await transactionDataProvider.Save(transaction);
        return transactionDataProvider.GetById(transaction.Id).Result;
    }

    public async Task<IEnumerable<Transaction>> GetAll(Guid? profileId)
    {
        return await transactionDataProvider.GetAll(profileId);
    }

    public async Task<IEnumerable<Transaction>> GetAll(Guid? profileId, Guid? userId)
    {
        return await transactionDataProvider.GetAll(profileId, userId);
    }

    public async Task<IEnumerable<Transaction>> GetAllCurrentMonth(Guid? profileId)
    {
        return await transactionDataProvider.GetAllCurrentMonth(profileId);
    }

    public async Task<IEnumerable<Transaction>> GetAllPreviousMonth(Guid? profileId)
    {
        return await transactionDataProvider.GetAllPreviousMonth(profileId);
    }

    public async Task<IEnumerable<Transaction>> GetAllCurrentMonth(Guid? profileId, Guid? userId)
    {
        return await transactionDataProvider.GetAllCurrentMonth(profileId, userId);
    }

    public async Task<decimal> GetTotalCurrentMonth(Guid? profileId)
    {
        IEnumerable<Transaction> transactions = await GetAllCurrentMonth(profileId);
        return CountTotal(transactions);
    }

    public async Task<decimal> GetTotalPreviousMonth(Guid? profileId)
    {
        IEnumerable<Transaction> transactions = await GetAllPreviousMonth(profileId);
        return CountTotal(transactions);
    }

    public async Task<string?> GetCurrentPayments(Guid? profileId)
    {
        IEnumerable<Transaction> transactions = await GetAllCurrentMonth(profileId);
        if (!transactions.Any()) return null;

        return await ConvertToString(transactions);
    }

    public async Task<string?> GetPreviousPayments(Guid? profileId)
    {
        IEnumerable<Transaction> transactions = await GetAllPreviousMonth(profileId);
        if (!transactions.Any()) return null;

        return await ConvertToString(transactions);
    }

    public async Task<string?> GetBalances(Guid? profileId)
    {
        if (profileId == null) throw new Exception(Messages.ArgumentsCanNotBeEmpty);
        IEnumerable<TgUserProfile> participants = _profileService.GetUserProfileByProfileId(profileId).Result;
        int? numberOfParticipants = participants?.Count();

        if (numberOfParticipants > 1)
        {
            StringBuilder balances = new StringBuilder();

            IEnumerable<Transaction> allTransactions = await GetAll(profileId);

            List<Transaction> paymentTransactions = allTransactions.Where(x => x.Type == (int)TransactionType.Payment).ToList();
            List<Transaction> repaymentTransactions = allTransactions.Where(x => x.Type == (int)TransactionType.Repayment).ToList();

            decimal sum = 0;
            paymentTransactions.ForEach(x => sum += x.Amount);

            decimal? prognosedParticipantAmount = sum / numberOfParticipants;

            foreach (TgUserProfile participant in participants)
            {
                decimal actualParticipantAmount = 0;
                paymentTransactions.Where(x => x.UserId == participant.UserId).ToList().ForEach(x => actualParticipantAmount += x.Amount);
                repaymentTransactions.Where(x => x.UserId == participant.UserId).ToList().ForEach(x => actualParticipantAmount += x.Amount);
                repaymentTransactions.Where(x => x.UserId != participant.UserId).ToList().ForEach(x => actualParticipantAmount -= x.Amount);
                string? username = _tgUserService.GetFirstNameById(participant.UserId).Result;
                decimal? difference = actualParticipantAmount - prognosedParticipantAmount;
                decimal? result = difference < 0 ? difference : 0;
                balances.Append(String.Format("{0}: {1}", username, result.ToString()));
                balances.Append('\n');
            }

            return balances.ToString();
        }
        else { return Messages.ThereIsOnlyOneUserInTheProfile; }
    }

    private async Task<string?> ConvertToString(IEnumerable<Transaction> transactions)
    {
        StringBuilder lastPayments = new StringBuilder();

        foreach (Transaction trans in transactions.OrderByDescending(x => x.Date))
        {
            string? username = await _tgUserService.GetFirstNameById(trans.UserId);
            lastPayments.Append(trans.ToString(username));
            lastPayments.Append('\n');
        }

        return lastPayments.ToString();
    }

    private decimal CountTotal(IEnumerable<Transaction> transactions)
    {
        if (!transactions.Any()) return 0;
        decimal total = 0;

        foreach (Transaction transaction in transactions)
        {
            if (transaction.Type == (int)TransactionType.Repayment) continue;
            total += transaction.Amount;
        }

        return total;
    }
}