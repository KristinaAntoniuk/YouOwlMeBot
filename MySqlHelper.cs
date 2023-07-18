using MySqlConnector;

public class MySqlHelper{
    MySqlDataProvider dataProvider;
    public MySqlHelper()
    {
        dataProvider = new MySqlDataProvider();
    }
    public int GetUserID(string username){
        
        return Convert.ToInt32(dataProvider.GetUserIDByUsername(username));
    }
}