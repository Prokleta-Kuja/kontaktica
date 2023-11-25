using System.Text.Json.Serialization;

namespace kontaktica.Models;

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