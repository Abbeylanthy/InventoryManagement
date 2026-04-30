using System.Net;
using System.Net.Mail;
using InventoryManagement.Entities;
using InventoryManagement.Services.Interfaces;
using Microsoft.Extensions.Options;

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
        // Configure SMTP client
        var smtpClient = new SmtpClient(_settings.SmtpServer)
        {
            //Port = 587, // Common SMTP port for secure email submission
            Port = _settings.Port,
            Credentials = new NetworkCredential(_settings.Username, _settings.Password),
            EnableSsl = true,
        };
// Create email message
        var mail = new MailMessage
        {
            // Set sender email and name
            From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mail.To.Add(toEmail);

        await smtpClient.SendMailAsync(mail);
    }
}
    
