using System.ComponentModel;
using System.Text;
using Hangfire;
using kontaktica.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace kontaktica.Jobs;

public class ModWeb
{
    readonly ILogger<ModWeb> _logger;
    public ModWeb(ILogger<ModWeb> logger)
    {
        _logger = logger;
    }

    [DisplayName("Send MetabuchMod contact email")]
    [AutomaticRetry(Attempts = 0)]
    public async Task RunAsync(ModWebRequest req, CancellationToken token)
    {
        _logger.LogInformation("Sending MetabuchMod contact email");
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
        bb.TextBody = GetPlainBody(req);
        email.Body = bb.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(conf.Host, conf.Port, SecureSocketOptions.Auto, token);
        await client.AuthenticateAsync(conf.Username, conf.Password, token);
        await client.SendAsync(email, token);
        await client.DisconnectAsync(true, token);
    }
    static string GetPlainBody(ModWebRequest req)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Ime i prezime: {req.Name}");
        sb.AppendLine($"Poslovni subjekt: {req.Subject}");
        sb.AppendLine($"Telefon: {req.Phone}");
        sb.AppendLine($"Email: {req.Email}");
        sb.AppendLine($"Poruka: {req.Message}");
        return sb.ToString();
    }
}