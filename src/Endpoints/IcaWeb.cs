using System.Text;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace kontaktica.Endpoints;

public static class IcaWebExtensions
{
    public static void MapIcaWeb(this WebApplication app)
    {
        app.MapPost(C.Routes.IcaWeb, async (IcaWebRequest data) =>
        {
            var conf = C.Settings.IcaWeb;

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
            await client.ConnectAsync(conf.Host, conf.Port, SecureSocketOptions.Auto);
            await client.AuthenticateAsync(conf.Username, conf.Password);
            await client.SendAsync(email);
            await client.DisconnectAsync(true);
        });
    }
    static string GetPlainBody(IcaWebRequest data)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"First name: {data.FirstName}");
        sb.AppendLine($"Last name: {data.LastName}");
        sb.AppendLine($"Company: {data.Company}");
        sb.AppendLine($"Time zone: {data.Tz}");
        sb.AppendLine($"Email: {data.Email}");
        sb.AppendLine($"Message: {data.Message}");
        return sb.ToString();
    }
}

public partial class IcaWebRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Company { get; set; }
    public string? Tz { get; set; }
    public string? Message { get; set; }
}