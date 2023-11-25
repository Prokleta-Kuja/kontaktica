using System.ComponentModel;
using System.Text;
using Hangfire;
using kontaktica.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace kontaktica.Jobs;

public class IcaWeb
{
    readonly ILogger<IcaWeb> _logger;
    public IcaWeb(ILogger<IcaWeb> logger)
    {
        _logger = logger;
    }

    [DisplayName("Send ICA contact email")]
    [AutomaticRetry(Attempts = 0)]
    public async Task RunAsync(IcaWebRequest req, CancellationToken token)
    {
        _logger.LogInformation("Sending ICA contact email");
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
        bb.TextBody = GetPlainBody(req);
        email.Body = bb.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(conf.Host, conf.Port, SecureSocketOptions.Auto, token);
        await client.AuthenticateAsync(conf.Username, conf.Password, token);
        await client.SendAsync(email, token);
        await client.DisconnectAsync(true, token);
    }
    static string GetPlainBody(IcaWebRequest req)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"First name: {req.FirstName}");
        sb.AppendLine($"Last name: {req.LastName}");
        sb.AppendLine($"Company: {req.Company}");
        sb.AppendLine($"Time zone: {req.Tz}");
        sb.AppendLine($"Email: {req.Email}");
        sb.AppendLine($"Message: {req.Message}");
        return sb.ToString();
    }
}