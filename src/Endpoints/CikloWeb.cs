using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace kontaktica.Endpoints;

public static class CikloWebExtensions
{
    const string MailTemplate = @"<!doctype html><html xmlns=""http://www.w3.org/1999/xhtml"" xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:o=""urn:schemas-microsoft-com:office:office""><head><title></title><!--[if !mso]><!-- --><meta http-equiv=""X-UA-Compatible"" content=""IE=edge""><!--<![endif]--><meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8""><meta name=""viewport"" content=""width=device-width,initial-scale=1""><style type=""text/css"">#outlook a { padding:0; }
          body { margin:0;padding:0;-webkit-text-size-adjust:100%;-ms-text-size-adjust:100%; }
          table, td { border-collapse:collapse;mso-table-lspace:0pt;mso-table-rspace:0pt; }
          img { border:0;height:auto;line-height:100%; outline:none;text-decoration:none;-ms-interpolation-mode:bicubic; }
          p { display:block;margin:13px 0; }</style><!--[if mso]>
        <xml>
        <o:OfficeDocumentSettings>
          <o:AllowPNG/>
          <o:PixelsPerInch>96</o:PixelsPerInch>
        </o:OfficeDocumentSettings>
        </xml>
        <![endif]--><!--[if lte mso 11]>
        <style type=""text/css"">
          .mj-outlook-group-fix { width:100% !important; }
        </style>
        <![endif]--><!--[if !mso]><!--><link href=""https://fonts.googleapis.com/css?family=Ubuntu:300,400,500,700"" rel=""stylesheet"" type=""text/css""><style type=""text/css"">@import url(https://fonts.googleapis.com/css?family=Ubuntu:300,400,500,700);</style><!--<![endif]--><style type=""text/css"">@media only screen and (min-width:480px) {
        .mj-column-per-100 { width:100% !important; max-width: 100%; }
      }</style><style type=""text/css""></style></head><body><div><!--[if mso | IE]><table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class="""" style=""width:600px;"" width=""600"" ><tr><td style=""line-height:0px;font-size:0px;mso-line-height-rule:exactly;""><![endif]--><div style=""margin:0px auto;max-width:600px;""><table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""width:100%;""><tbody><tr><td style=""direction:ltr;font-size:0px;padding:20px 0;text-align:center;""><!--[if mso | IE]><table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0""><tr><td class="""" style=""vertical-align:top;width:600px;"" ><![endif]--><div class=""mj-column-per-100 mj-outlook-group-fix"" style=""font-size:0px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%;""><table border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""vertical-align:top;"" width=""100%""><tr><td align=""left"" style=""font-size:0px;padding:10px 25px;word-break:break-word;""><table cellpadding=""0"" cellspacing=""0"" width=""100%"" border=""0"" style=""color:#000000;font-family:Ubuntu, Helvetica, Arial, sans-serif;font-size:13px;line-height:22px;table-layout:auto;width:100%;border:none;""><tr style=""border-bottom:1px dotted;text-align:left;""><td>Način plaćanja</td><td>[[Payment]]</td></tr><tr style=""border-bottom:1px dotted;text-align:left;""><td>Ime</td><td>[[FirstName]]</td></tr><tr style=""border-bottom:1px dotted;text-align:left;""><td>Prezime</td><td>[[LastName]]</td></tr><tr style=""border-bottom:1px dotted;text-align:left;""><td>Email</td><td>[[Email]]</td></tr><tr style=""border-bottom:1px dotted;text-align:left;""><td>Kontakt</td><td>[[Tel]]</td></tr><tr style=""border-bottom:1px dotted;text-align:left;""><td>Adresa</td><td>[[Address]]</td></tr><tr style=""border-bottom:1px dotted;text-align:left;""><td>Mjesto</td><td>[[City]]</td></tr><tr style=""border-bottom:1px dotted;text-align:left;""><td>Poštanski broj</td><td>[[Zip]]</td></tr><tr style=""border-bottom:1px dotted;text-align:left;""><td>Napomena</td><td>[[Note]]</td></tr></table></td></tr></table></div><!--[if mso | IE]></td></tr></table><![endif]--></td></tr></tbody></table></div><!--[if mso | IE]></td></tr></table><table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class="""" style=""width:600px;"" width=""600"" ><tr><td style=""line-height:0px;font-size:0px;mso-line-height-rule:exactly;""><![endif]--><div style=""margin:0px auto;max-width:600px;""><table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""width:100%;""><tbody><tr><td style=""direction:ltr;font-size:0px;padding:20px 0;text-align:center;""><!--[if mso | IE]><table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0""><tr><td class="""" style=""vertical-align:top;width:600px;"" ><![endif]--><div class=""mj-column-per-100 mj-outlook-group-fix"" style=""font-size:0px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%;""><table border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""vertical-align:top;"" width=""100%""><tr><td align=""left"" style=""font-size:0px;padding:10px 25px;word-break:break-word;""><table cellpadding=""0"" cellspacing=""0"" width=""100%"" border=""0"" style=""color:#000000;font-family:Ubuntu, Helvetica, Arial, sans-serif;font-size:13px;line-height:22px;table-layout:auto;width:100%;border:none;""><tr style=""border-bottom:1px solid #ecedee;text-align:left;""><th>Šifra</th><th>Naziv</th><th>Količina</th></tr>[[Rows]]</table></td></tr></table></div><!--[if mso | IE]></td></tr></table><![endif]--></td></tr></tbody></table></div><!--[if mso | IE]></td></tr></table><![endif]--></div></body></html>";
    const string MailTemplateProductRow = @"<tr style=""text-align:left;""><td>[[Sku]]</td><td><a rel=""noopener noreferrer"" target=""_blank"" href=""[[Slug]]"" title=""[[Price]]"">[[Title]]</a></td><td>[[Quantity]]</td></tr>";

    public static void MapCikloWeb(this WebApplication app)
    {
        app.MapPost(C.Routes.CikloWeb, async (CikloWebRequest data) =>
        {
            if (data.Contact == null)
                throw new ArgumentNullException(nameof(data), "Contact can't be null");
            if (data.Items == null)
                throw new ArgumentNullException(nameof(data), "Items can't be null");

            var conf = C.Settings.CikloWeb;

            var productLines = GetProductLines(data.Items);

            var email = new MimeMessage();
            email.Subject = "Novi zahtjev za ponudu";

            email.From.Add(MailboxAddress.Parse(conf.From));

            foreach (var to in conf.To)
                email.To.Add(MailboxAddress.Parse(to));

            foreach (var cc in conf.Cc)
                email.Cc.Add(MailboxAddress.Parse(cc));

            foreach (var bcc in conf.Bcc)
                email.Bcc.Add(MailboxAddress.Parse(bcc));

            var bb = new BodyBuilder();
            bb.HtmlBody = GetHtmlBody(data.Contact, productLines);
            bb.TextBody = GetPlainBody(data);
            email.Body = bb.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(conf.Host, conf.Port, SecureSocketOptions.Auto);
            await client.SendAsync(email);
            await client.DisconnectAsync(true);
        });
    }
    static string GetPlainBody(CikloWebRequest data)
    {
        var sb = new StringBuilder();
        if (data.Contact != null)
        {
            sb.AppendLine($"Ime: {data.Contact.Payment}");
            sb.AppendLine($"Prezime: {data.Contact.Payment}");
            sb.AppendLine($"Email: {data.Contact.Payment}");
            sb.AppendLine($"Kontakt: {data.Contact.Payment}");
            sb.AppendLine($"Adresa: {data.Contact.Payment}");
            sb.AppendLine($"Poštanski broj: {data.Contact.Payment}");
            sb.AppendLine($"Mjesto: {data.Contact.Payment}");
            sb.AppendLine($"Način plaćanja: {data.Contact.Payment}");
            sb.AppendLine($"Napomena: {data.Contact.Payment}");
        }

        if (data.Items != null)
            foreach (var item in data.Items)
            {
                sb.AppendLine("-------------------");
                sb.AppendLine($"{item.Quantity}x {item.Title} (https://ciklo-sport.hr{item.Slug})");
                sb.AppendLine($"");
            }

        return sb.ToString();
    }
    static string GetHtmlBody(CikloWebContact c, string productLines)
    {
        var replacements = new Dictionary<string, string>();
        replacements.Add(nameof(c.Payment), c.Payment);
        replacements.Add(nameof(c.FirstName), c.FirstName);
        replacements.Add(nameof(c.LastName), c.LastName);
        replacements.Add(nameof(c.Email), c.Email);
        replacements.Add(nameof(c.Tel), c.Tel);
        replacements.Add(nameof(c.Address), c.Address);
        replacements.Add(nameof(c.City), c.City);
        replacements.Add(nameof(c.Zip), c.Zip);
        replacements.Add(nameof(c.Note), c.Note);
        replacements.Add("Rows", productLines);

        return Regex.Replace(MailTemplate, @"\[\[(.+?)\]\]", m => replacements[m.Groups[1].Value]);
    }

    static string GetProductLines(CikloWebItem[] items)
    {
        var sb = new StringBuilder();
        foreach (var item in items)
        {
            var replacements = new Dictionary<string, string>();
            replacements.Add(nameof(item.Title), item.Title ?? string.Empty);
            replacements.Add(nameof(item.Sku), item.Sku ?? string.Empty);
            replacements.Add(nameof(item.Slug), "https://ciklo-sport.hr" + item.Slug);
            replacements.Add(nameof(item.Price), item.Price.ToString());
            replacements.Add(nameof(item.CardPrice), item.CardPrice.ToString());
            replacements.Add(nameof(item.Quantity), item.Quantity.ToString());

            sb.Append(Regex.Replace(MailTemplateProductRow, @"\[\[(.+?)\]\]", m => replacements[m.Groups[1].Value]));
        }

        return sb.ToString();
    }
}

public partial class CikloWebRequest
{
    [JsonPropertyName("contact")] public CikloWebContact? Contact { get; set; }

    [JsonPropertyName("items")] public CikloWebItem[]? Items { get; set; }
}

public partial class CikloWebContact
{
    [JsonPropertyName("payment")] public string Payment { get; set; } = string.Empty;

    [JsonPropertyName("firstName")] public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("lastName")] public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("email")] public string Email { get; set; } = string.Empty;

    [JsonPropertyName("tel")] public string Tel { get; set; } = string.Empty;

    [JsonPropertyName("address")] public string Address { get; set; } = string.Empty;

    [JsonPropertyName("city")] public string City { get; set; } = string.Empty;

    [JsonPropertyName("zip")] public string Zip { get; set; } = string.Empty;

    [JsonPropertyName("note")] public string Note { get; set; } = string.Empty;
}

public partial class CikloWebItem
{
    [JsonPropertyName("title")] public string? Title { get; set; }
    [JsonPropertyName("sku")] public string? Sku { get; set; }

    [JsonPropertyName("slug")] public string? Slug { get; set; }

    [JsonPropertyName("price")] public double Price { get; set; }

    [JsonPropertyName("cardPrice")] public double CardPrice { get; set; }

    [JsonPropertyName("quantity")] public int Quantity { get; set; }
}