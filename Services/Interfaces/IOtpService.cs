namespace InventoryManagement.Services.Interfaces;
public interface IOtpService
{
    Task<string> GenerateOtpAsync(string email);
    Task<bool> VerifyOtpAsync(string email, string otp);
    Task<bool> ResendOtpAsync(string email);
}