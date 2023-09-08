using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using YouOwlMeBot.Custom;
using YouOwlMeBot.Models;

namespace YouOwlMeBot.DataProviders;

public class TransactionDataProvider : ContextProvider, IDataProvider<Transaction>
{
    public TransactionDataProvider(IDynamoDBContext dynamoDBContext) : base(dynamoDBContext) { }

    public async Task<IEnumerable<Transaction>> GetAll()
    {
        return await _dynamoDBContext.ScanAsync<Transaction>(default).GetRemainingAsync();
    }

    public async Task<IEnumerable<Transaction>> GetAll(Guid? profileId)
    {
        if (profileId == Guid.Empty) throw new Exception(Messages.ArgumentsCanNotBeEmpty);

        IEnumerable<ScanCondition> conditions = new List<ScanCondition>
        {
            new ScanCondition("ProfileId", ScanOperator.Equal, profileId)
        };

        return await _dynamoDBContext.ScanAsync<Transaction>(conditions).GetRemainingAsync();
    }

    public async Task<IEnumerable<Transaction>> GetAll(Guid? profileId, Guid? userId)
    {
        if (profileId == Guid.Empty || userId == Guid.Empty) throw new Exception(Messages.ArgumentsCanNotBeEmpty);

        IEnumerable<ScanCondition> conditions = new List<ScanCondition>
        {
            new ScanCondition("ProfileId", ScanOperator.Equal, profileId),
            new ScanCondition("UserId", ScanOperator.Equal, userId)
        };

        return await _dynamoDBContext.ScanAsync<Transaction>(conditions).GetRemainingAsync();
    }

    public async Task<Transaction> GetById(Guid? guid)
    {
        return await _dynamoDBContext.LoadAsync<Transaction>(guid);
    }

    public async void Delete(Transaction data)
    {
        await _dynamoDBContext.DeleteAsync(data);
    }

    public async Task Save(Transaction data)
    {
        await _dynamoDBContext.SaveAsync(data);
    }

    internal async Task<IEnumerable<Transaction>> GetAllCurrentMonth(Guid? profileId)
    {
        if (profileId == Guid.Empty) throw new Exception(Messages.ArgumentsCanNotBeEmpty);

        IEnumerable<ScanCondition> conditions = new List<ScanCondition>
        {
            new ScanCondition("Date", ScanOperator.GreaterThanOrEqual, new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)),
            new ScanCondition("ProfileId", ScanOperator.Equal, profileId)
        };

        return await _dynamoDBContext.ScanAsync<Transaction>(conditions).GetRemainingAsync();
    }

    internal async Task<IEnumerable<Transaction>> GetAllPreviousMonth(Guid? profileId)
    {
        if (profileId == Guid.Empty) throw new Exception(Messages.ArgumentsCanNotBeEmpty);

        IEnumerable<ScanCondition> conditions = new List<ScanCondition>
        {
            new ScanCondition("Date", ScanOperator.GreaterThanOrEqual, new DateTime(DateTime.Now.Year, DateTime.Now.Month-1, 1)),
            new ScanCondition("Date", ScanOperator.LessThan, new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)),
            new ScanCondition("ProfileId", ScanOperator.Equal, profileId)
        };

        return await _dynamoDBContext.ScanAsync<Transaction>(conditions).GetRemainingAsync();
    }

    internal async Task<IEnumerable<Transaction>> GetAllCurrentMonth(Guid? profileId, Guid? userId)
    {
        if (profileId == Guid.Empty || userId == Guid.Empty) throw new Exception(Messages.ArgumentsCanNotBeEmpty);

        IEnumerable<ScanCondition> conditions = new List<ScanCondition>
        {
            new ScanCondition("Date", ScanOperator.GreaterThanOrEqual, new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)),
            new ScanCondition("ProfileId", ScanOperator.Equal, profileId),
            new ScanCondition("UserId", ScanOperator.Equal, userId)
        };

        return await _dynamoDBContext.ScanAsync<Transaction>(conditions).GetRemainingAsync();
    }
}
