[System.Serializable]
public class User
{
    public string email;
    public string username;
    public string password;

    public User(string email, string username, string password)
    {
        this.email = email;
        this.username = username;
        this.password = password;
    }
}

