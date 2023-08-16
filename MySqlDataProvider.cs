using misha_kris_finance_bot;
using misha_kris_finance_bot.Models;

public class MySqlDataProvider
{

    private DataContext dataContext;

    public MySqlDataProvider()
    {
        dataContext = new DataContext();
    }

    public int? GetUserIDByUsername(string? username)
    {
        if (username == null) {  return null; }
        else
        {
            User? user = dataContext.Users.FirstOrDefault(x => x.Username == username);
            return user?.Id;
        }
    }

    public int? AddUser(string? username)
    {
        if (username == null) { return null; }
        else
        {
            User user = new User()
            {
                Username = username
            };
            dataContext.Users.Add(user);
            dataContext.SaveChanges();

            return GetUserIDByUsername(username);
        }
    }

    public int? AddTransaction (Transaction? transaction)
    {
        if (transaction == null) { return null; }
        else
        {
            dataContext.Transactions.Add(transaction);
            dataContext.SaveChanges();
            return GetTransactionByGuid(transaction.Guid);
        }
    }

    public int? GetTransactionByGuid(Guid? guid)
    {
        if (guid == null) { return null;}
        else
        {
            Transaction? transaction = dataContext.Transactions.FirstOrDefault(x => x.Guid == guid);
            return transaction?.Id;
        }
    }
}