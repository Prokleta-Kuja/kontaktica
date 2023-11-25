using System.Text;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace kontaktica.Endpoints;

public static class ModWebExtensions
{
    public static void MapModWeb(this WebApplication app)
    {
        app.MapPost(C.Routes.ModWeb, async (ModWebRequest data) =>
        {
            var conf = C.Settings.ModWeb;

            var email = new MimeMessage();
            email.Subject = "Upit sa weba";

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
    static string GetPlainBody(ModWebRequest data)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Ime i prezime: {data.Name}");
        sb.AppendLine($"Poslovni subjekt: {data.Subject}");
        sb.AppendLine($"Telefon: {data.Phone}");
        sb.AppendLine($"Email: {data.Email}");
        sb.AppendLine($"Poruka: {data.Message}");
        return sb.ToString();
    }
}

public partial class ModWebRequest
{
    public string? Name { get; set; }
    public string? Subject { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Message { get; set; }
}