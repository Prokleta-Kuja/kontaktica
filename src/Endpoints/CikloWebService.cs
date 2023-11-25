using System.Text;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace kontaktica.Endpoints;

public static class CikloWebServiceExtensions
{
    public static void MapCikloWebService(this WebApplication app)
    {
        app.MapPost(C.Routes.CikloWebService, async (CikloWebServiceRequest data) =>
        {
            var conf = C.Settings.CikloWeb;

            var email = new MimeMessage();
            email.Subject = "🛠️ Naručivanje sa weba 🛠️";

            email.From.Add(MailboxAddress.Parse(conf.From));

            foreach (var to in conf.To)
                email.To.Add(MailboxAddress.Parse(to));

            foreach (var cc in conf.Cc)
                email.Cc.Add(MailboxAddress.Parse(cc));

            foreach (var bcc in conf.Bcc)
                email.Bcc.Add(MailboxAddress.Parse(bcc));

            var bb = new BodyBuilder();
            bb.TextBody = GetPlainBody(data);
            email.Body = bb.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(conf.Host, conf.Port, SecureSocketOptions.SslOnConnect);
            await client.AuthenticateAsync(conf.Username, conf.Password);
            await client.SendAsync(email);
            await client.DisconnectAsync(true);
        });
    }
    static string GetPlainBody(CikloWebServiceRequest data)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Subjekt: {data.Subject}");
        sb.AppendLine($"Ime: {data.FirstName}");
        sb.AppendLine($"Prezime: {data.LastName}");
        sb.AppendLine($"Telefon: {data.Tel}");
        sb.AppendLine($"Email: {data.Email}");
        sb.AppendLine($"Dan: {data.Date}");
        sb.AppendLine($"Napomena: {data.Note}");
        sb.AppendLine();
        if (data.Services != null)
            foreach (var service in data.Services)
                sb.AppendLine($"{service.Value}x {service.Key}");

        return sb.ToString();
    }
}

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