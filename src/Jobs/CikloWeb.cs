using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using Hangfire;
using kontaktica.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace kontaktica.Jobs;

public partial class CikloWeb
{
    readonly ILogger<CikloWeb> _logger;
    public CikloWeb(ILogger<CikloWeb> logger)
    {
        _logger = logger;
    }

    [DisplayName("Send Ciklo-Sport contact email")]
    [AutomaticRetry(Attempts = 0)]
    public async Task RunAsync(CikloWebRequest req, CancellationToken token)
    {
        _logger.LogInformation("Sending Ciklo-Sport contact email");
        if (req.Contact == null)
            throw new ArgumentNullException(nameof(req), "Contact can't be null");
        if (req.Items == null)
            throw new ArgumentNullException(nameof(req), "Items can't be null");

        var conf = C.Settings.CikloWeb;

        var email = new MimeMessage();
        email.Subject = "🎉 Novi zahtjev za ponudu 🎉";

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
    static string GetPlainBody(CikloWebRequest req)
    {
        var sb = new StringBuilder();
        if (req.Contact != null)
        {
            sb.AppendLine($"Ime: {req.Contact.FirstName}");
            sb.AppendLine($"Prezime: {req.Contact.LastName}");
            sb.AppendLine($"Email: {req.Contact.Email}");
            sb.AppendLine($"Kontakt: {req.Contact.Tel}");
            sb.AppendLine($"Adresa: {req.Contact.Address}");
            sb.AppendLine($"Poštanski broj: {req.Contact.Zip}");
            sb.AppendLine($"Mjesto: {req.Contact.City}");
            sb.AppendLine($"Način plaćanja: {req.Contact.Payment}");
            sb.AppendLine($"Napomena: {req.Contact.Note}");
        }

        if (req.Items != null)
            foreach (var item in req.Items)
            {
                sb.AppendLine("-------------------");
                sb.AppendLine($"{item.Quantity}x {item.Title} (https://ciklo-sport.hr{item.Slug})");
                sb.AppendLine($"");
            }

        return sb.ToString();
    }
}