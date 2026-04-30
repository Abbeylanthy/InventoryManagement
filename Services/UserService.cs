using Microsoft.AspNetCore.Identity;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.DTOs.User;
using InventoryManagement.Enum;
using InventoryManagement.Repositories.Interfaces;
using InventoryManagement.Entities;
using InventoryManagement.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using InventoryManagement.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
namespace InventoryManagement.Services.Interfaces;
public class UserService : IUserService
{
    private readonly IGenericRepository<User> _repository;
    private readonly IMapper _mapper;
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IOtpService _otpService;
    private readonly IHttpContextAccessor _httpContextAccessor;

public UserService (IGenericRepository<User> repository, IMapper mapper, AppDbContext _context, IPasswordHasher<User> passwordHasher, IOtpService otpService, IHttpContextAccessor httpContextAccessor)
    {
        _repository = repository;
        _mapper = mapper;
        this._context = _context;
        this._passwordHasher = passwordHasher;
        this._otpService = otpService;
        this._httpContextAccessor = httpContextAccessor;
    }
    public async Task<UserDto> CreateUserAsync(UserCreateDto dto)
    {
        // Validation
        if (string.IsNullOrEmpty(dto.UserName) ||
         string.IsNullOrEmpty(dto.Email) ||
          string.IsNullOrEmpty(dto.Password))
        {
            throw new ArgumentException("Username, Email and Password are required.");
        }
        
        // Minimum Length
        if (dto.Password.Length < 8)
        {
            throw new InvalidOperationException("Password must be at least 8 characters long");
        }

        // At least 1 UpperCase
        if (!dto.Password.Any(char.IsUpper))
        {
            throw new InvalidOperationException("Password must contain at least 1 Uppercase letter");
        }
        
        // At least 1 special character
        if (!dto.Password.Any(ch => ! char.IsLetterOrDigit(ch)))
        {
            throw new InvalidOperationException("Password must contain at least 1 special character");
        }

        // Check if user exists
        if (await _context.Users.AnyAsync(u => u.UserName == dto.UserName))
        {
            throw new InvalidOperationException("User with the same username or email already exists.");
        }
        // Create new user entity
        
        var user = new User
        {
            UserName = dto.UserName,
            Email = dto.Email,
            Role = dto.Role,  
            IsActive = false,
            EmailVerified = false  
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        await _otpService.GenerateOtpAsync(user.Email);
        return new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Role = user.Role
        };
    }
    public async Task<UserDto> LoginAsync(UserLoginDto dto)
{
    var user = await _context.Users
        .FirstOrDefaultAsync(u => u.UserName == dto.UserName);

    if (user == null)
        throw new InvalidOperationException("Invalid username or password.");

        if (!user.EmailVerified)
        throw new InvalidOperationException("Please verify your email before logging in.");

         //  BLOCK INACTIVE USERS HERE
    if (!user.IsActive)
        throw new InvalidOperationException("Account is deactivated");


var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, dto.Password);
if (result == PasswordVerificationResult.Failed)
    throw new InvalidOperationException("Invalid username or password.");

    return _mapper.Map<UserDto>(user);
}
    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _context.Users.ToListAsync();
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }
    public async Task<bool> DeleteUserAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return false; // User not found
        }
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true; // User deleted successfully
    }
    public async Task<bool> DeactivateAccountAsync(int userId)
    {
        
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return false; // User not found
        }
        if (!user.IsActive)
        {
            return true; // User already deactivated, consider as success
        }
        user.IsActive = false;
        await _context.SaveChangesAsync();
        return true; // User deactivated successfully

}
public async Task ForgetPasswordAsync(string email)
    {
        // Check if user exists and is verified
        var user = await _context.Users
        .FirstOrDefaultAsync(u => u.Email == email);

    if (user == null)
        throw new Exception("User not found");

    if (!user.EmailVerified)
        throw new Exception("Email is not verified");

// Generate OTP and send email
    await _otpService.GenerateOtpAsync(email);
    }
    public async Task ResetPasswordAsync(ResetPasswordDto dto)
{
    // Verify OTP first
    var isValidOtp = await _otpService.VerifyOtpAsync(dto.Email, dto.Otp);

    if (!isValidOtp)
        throw new Exception("Invalid or expired OTP");

    var user = await _context.Users
        .FirstOrDefaultAsync(u => u.Email == dto.Email);

    if (user == null)
        throw new Exception("User not found");

    //  Optional: prevent same password reuse
    var samePassword = _passwordHasher.VerifyHashedPassword(
        user,
        user.PasswordHash!,
        dto.NewPassword
    );

    if (samePassword != PasswordVerificationResult.Failed)
        throw new Exception("New password cannot be the same as old password");

    // Hash new password
    user.PasswordHash = _passwordHasher.HashPassword(user, dto.NewPassword);

    await _context.SaveChangesAsync();
}
public async Task ChangePasswordAsync(ChangePasswordDto dto)
{
    // Get user ID from JWT claims
    var userId = _httpContextAccessor.HttpContext!.User
        .FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (userId == null)
        throw new Exception("User not found in token");

    var user = await _context.Users
        .FirstOrDefaultAsync(u => u.Id == int.Parse(userId));

    if (user == null)
        throw new Exception("User not found");

    // 1. Verify current password
    var result = _passwordHasher.VerifyHashedPassword(
        user,
        user.PasswordHash!,
        dto.CurrentPassword
    );

    if (result == PasswordVerificationResult.Failed)
        throw new Exception("Current password is incorrect");

    // 2. Prevent reuse of same password
    var samePassword = _passwordHasher.VerifyHashedPassword(
        user,
        user.PasswordHash!,
        dto.NewPassword
    );

    if (samePassword != PasswordVerificationResult.Failed)
        throw new Exception("New password cannot be the same as old password");

    // 3. Hash and update password
    user.PasswordHash = _passwordHasher.HashPassword(user, dto.NewPassword);

    await _context.SaveChangesAsync();
}
}
    

    
    