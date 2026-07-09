using InventoryManagement.Entities;
using InventoryManagement.Services.Interfaces;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace InventoryManagement.Services;
public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    public EmailService(IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            Console.WriteLine($"[EMAIL] Attempting to send email to: {toEmail}");
            Console.WriteLine($"[EMAIL] From: {_settings.SenderEmail}");
            
            var client = new SendGridClient(_settings.SendGridApiKey);
            var from = new EmailAddress(_settings.SenderEmail, _settings.SenderName);
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, subject, body);
            
            Console.WriteLine($"[EMAIL] Sending via SendGrid...");
            var response = await client.SendEmailAsync(msg);
            
            Console.WriteLine($"[EMAIL] SendGrid Response Status: {response.StatusCode}");
            Console.WriteLine($"[EMAIL] Is Success: {response.IsSuccessStatusCode}");
            
            if (!response.IsSuccessStatusCode)
            {
                var body_response = await response.Body.ReadAsStringAsync();
                Console.WriteLine($"[EMAIL] Error Response: {body_response}");
                throw new Exception($"SendGrid error: {response.StatusCode} - {body_response}");
            }
            
            Console.WriteLine($"[EMAIL] Email sent successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[EMAIL] FAILED to send email to {toEmail}: {ex.Message}");
            Console.WriteLine($"[EMAIL] Exception Details: {ex.InnerException?.Message}");
            throw new Exception($"Failed to send email to {toEmail}: {ex.Message}", ex);
        }
    }
}
    
