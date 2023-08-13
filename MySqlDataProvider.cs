using MySqlConnector;
public class MySqlDataProvider
{
    
    private string connectionString;

    public MySqlDataProvider(string host, string schema, string username, string password)
    {
        this.connectionString = "Server=" + host + ";User ID=" + username + ";Password=" + password + ";Database=" + schema;
    }

    public MySqlDataProvider(string schema, string username, string password) : this ("127.0.0.1", schema, username, password){}

    public MySqlDataProvider(string username, string password) : this("127.0.0.1", "financedb", username, password){}

    public MySqlDataProvider() : this("127.0.0.1", "financedb", "root", "admin"){}

    public int? GetUserIDByUsername(string? username)
    {
        if (username == null) return null;
        try
        {
            object? result = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand("SELECT ID FROM financedb.user WHERE username = '" + username + "'", connection);
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result = reader.GetValue(0);
                }
            }
            return result == null ? null : Convert.ToInt32(result);
        }
        catch (Exception ex){
            throw ex;
        }
    }
    public bool AddTransaction(int? userID, string? store, decimal? amount)
    {
        if (userID == null || store == null || amount == null) return false;
        try
        {
            int rows = 0;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand("INSERT INTO `financedb`.`transaction` (`UserID`,`Date`,`Store`,`Amount`) VALUES ('" + userID + "', '" + DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss") + "', '" + store + "', '" + amount + "');", connection);
                rows = command.ExecuteNonQuery();
            }
                
            return rows > 0;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public bool AddUser(string? username)
    {
        if (username == null) return false;
        try
        {
            int rows = 0;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand("INSERT INTO `financedb`.`user` (`username`) VALUES ( '" + username + "');", connection);
                rows = command.ExecuteNonQuery();
            }
                
            return rows > 0;
        }
        catch (Exception ex){
            throw ex;
        }
    }
}