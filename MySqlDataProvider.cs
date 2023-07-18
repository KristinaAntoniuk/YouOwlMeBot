using MySqlConnector;
public class MySqlDataProvider{
    
    private string connectionString;
    MySqlConnection connection;

    public MySqlDataProvider(string host, string schema, string username, string password)
    {
        this.connectionString = "Server=" + host + ";User ID=" + username + ";Password=" + password + ";Database=" + schema;
        connection = new MySqlConnection(connectionString);
    }

    public MySqlDataProvider(string schema, string username, string password) : this ("127.0.0.1", schema, username, password){}

    public MySqlDataProvider(string username, string password) : this("127.0.0.1", "financedb", username, password){}

    public MySqlDataProvider() : this("127.0.0.1", "financedb", "root", "admin"){}

    public object? GetUserIDByUsername(string username){
        try{
            connection.Open();
            MySqlCommand command =  new MySqlCommand("SELECT ID FROM User WHERE Name = \'" + username + "\'", connection);
            MySqlDataReader reader = command.ExecuteReader();
            object? result = null;
            while (reader.Read()){
                result = reader.GetValue(0);
            }
            connection.Close();
            return result;
        }
        catch (Exception ex){
            throw ex;
        }
    }

    public bool AddUser(string username){
        try{
            connection.Open();
            MySqlCommand command =  new MySqlCommand("INSERT INTO `financedb`.`user` (`Name`) VALUES (" + username + ");");
            int rows = command.ExecuteNonQuery();
            return rows > 0;
        }
        catch (Exception ex){
            throw ex;
        }
    }
}