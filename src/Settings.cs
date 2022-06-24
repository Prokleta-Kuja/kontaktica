namespace kontaktica;

public class Settings
{
    public List<string> Origins { get; set; } = new() { "https://example.com", "https://www.example.com" };
    public GenericMailService CikloWeb { get; set; } = new("host", 587, "user", "pass", "First Last <first.last@example.com>");
    public GenericMailService IcaWeb { get; set; } = new("host", 587, "user", "pass", "First Last <first.last@example.com>");
}

public class GenericMailService
{
    public GenericMailService(string host, int port, string username, string password, string from)
    {
        Host = host;
        Port = port;
        Username = username;
        Password = password;
        From = from;
    }

    public string Host { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string From { get; set; }
    public List<string> To { get; set; } = new();
    public List<string> Cc { get; set; } = new();
    public List<string> Bcc { get; set; } = new();
}