namespace InventoryManagement.Entities;
public class EmailSettings
{
    public string SendGridApiKey { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string SenderEmail { get; set; } = string.Empty;
}