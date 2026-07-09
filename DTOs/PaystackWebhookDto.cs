public class PaystackWebhookDto
{
    public string eventType { get; set; } = string.Empty;
    public PaystackWebhookData data { get; set; } = null!;
}

public class PaystackWebhookData
{
    public string reference { get; set; } = string.Empty;
    public string status { get; set; } = string.Empty;
    public decimal amount { get; set; }
    public string gateway_response { get; set; } = string.Empty;
}