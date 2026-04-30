using InventoryManagement.Data;
using InventoryManagement.Entities;
using InventoryManagement.Services.Interfaces;

namespace InventoryManagement.Services;
public class OtpService : IOtpService
{
    
    private readonly AppDbContext _context;
    private readonly IEmailService _emailService;
    public OtpService(AppDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }
    // Helper method to generate a 6-digit OTP
    private string GenerateCode()
    {
        var random = new Random();
        return random.Next(100000, 999999).ToString();
    }
    public async Task<string> GenerateOtpAsync(string email)
    {
        // Generate OTP and save to database
        var otp = GenerateCode();
        var record = new EmailVerificationOtp
        {
            // Id will be auto-generated
            Email = email,
            OtpCode = otp,
            ExpiryTime = DateTime.UtcNow.AddMinutes(5),
            IsUsed = false
        };
        _context.EmailVerificationOtps.Add(record);
        await _context.SaveChangesAsync();
        await _emailService.SendEmailAsync(email, "Email Verification OTP", $"Your OTP is: <b>{otp}</b>. It will exppire in 5 minutes");
        return otp; 
    }

    public async Task<bool> VerifyOtpAsync(string email, string otp)
    {
        // Check if OTP exists and is valid
        var record = _context.EmailVerificationOtps.FirstOrDefault(r => r.Email == email && r.OtpCode == otp);
        if (record == null)
        return false;
        if (record.IsUsed)
        return false;
        if (record.ExpiryTime < DateTime.UtcNow)
        return false;
// Mark OTP as used and activate user
        var user = _context.Users.FirstOrDefault(r => r.Email == email);
        if (user == null)
        return false;
        user.IsActive = true;
        user.EmailVerified = true;
        record.IsUsed = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ResendOtpAsync(string email)
    {
        // Check if user exists and is not already verified
        var user = _context.Users.FirstOrDefault(u => u.Email == email);

        if (user == null)
            return false;
            // Don't resend if already verified
        if (user.EmailVerified)
            return false;
            // Invalidate old OTPs
        var oldOtps = _context.EmailVerificationOtps.Where(o => o.Email == email && !o.IsUsed);
        foreach (var otp in oldOtps)
        {
            otp.IsUsed = true;
        }
        // Generate new OTP
        var newOtp = new Random().Next(100000, 999999).ToString();
        var otpEntity = new EmailVerificationOtp
        {
            Email = email,
            OtpCode = newOtp,
            ExpiryTime = DateTime.UtcNow.AddMinutes(5),
            IsUsed = false
        };
        _context.EmailVerificationOtps.Add(otpEntity);
        await _context.SaveChangesAsync();
        return true;
    }
}
