using System.ComponentModel;
using System.Text;
using Hangfire;
using kontaktica.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace kontaktica.Jobs;

public class CikloWebService
{
    readonly ILogger<CikloWebService> _logger;
    public CikloWebService(ILogger<CikloWebService> logger)
    {
        _logger = logger;
    }

    [DisplayName("Send Ciklo-Sport service request email")]
    [AutomaticRetry(Attempts = 0)]
    public async Task RunAsync(CikloWebServiceRequest req, CancellationToken token)
    {
        _logger.LogInformation("Sending Ciklo-Sport service request email");
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
        bb.TextBody = GetPlainBody(req);
        email.Body = bb.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(conf.Host, conf.Port, SecureSocketOptions.Auto, token);
        await client.AuthenticateAsync(conf.Username, conf.Password, token);
        await client.SendAsync(email, token);
        await client.DisconnectAsync(true, token);
    }
    static string GetPlainBody(CikloWebServiceRequest req)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Subjekt: {req.Subject}");
        sb.AppendLine($"Ime: {req.FirstName}");
        sb.AppendLine($"Prezime: {req.LastName}");
        sb.AppendLine($"Telefon: {req.Tel}");
        sb.AppendLine($"Email: {req.Email}");
        sb.AppendLine($"Dan: {req.Date}");
        sb.AppendLine($"Napomena: {req.Note}");
        sb.AppendLine();
        if (req.Services != null)
            foreach (var service in req.Services)
                sb.AppendLine($"{service.Value}x {service.Key}");

        return sb.ToString();
    }
}