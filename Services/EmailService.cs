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
    try
    {
        var smtpClient = new SmtpClient(_settings.SmtpServer)
        {
            Port = _settings.Port,
            Credentials = new NetworkCredential(_settings.Username, _settings.Password),
            EnableSsl = true,
        };

        var mail = new MailMessage
        {
            From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mail.To.Add(toEmail);

        await smtpClient.SendMailAsync(mail);
    }
    catch (Exception ex)
    {
        // THIS is what will show the REAL error
        throw new Exception(ex.ToString());
    }
}
}
    
