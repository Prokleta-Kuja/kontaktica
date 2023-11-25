namespace kontaktica.Models;

public partial class CikloWebServiceRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Tel { get; set; }
    public string? Subject { get; set; }
    public string? Note { get; set; }
    public string? Date { get; set; }
    public Dictionary<string, int>? Services { get; set; }
}