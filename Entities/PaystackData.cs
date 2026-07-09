using System.Text.Json.Serialization;

public class PaystackData
{
    [JsonPropertyName("authorization_url")]
    public string? AuthorizationUrl { get; set; } = string.Empty;

    [JsonPropertyName("access_code")]
    public string? AccessCode { get; set; } = string.Empty;

    [JsonPropertyName("reference")]
    public string Reference { get; set; } = string.Empty;
}