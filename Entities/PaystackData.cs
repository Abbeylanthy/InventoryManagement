using System.Text.Json.Serialization;

public class PaystackData
{
    public string? AuthorizationUrl { get; set; } = string.Empty;
    public string? AccessCode { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
}