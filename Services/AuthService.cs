 using System.Security.Claims;
 using InventoryManagement.Entities;
 using InventoryManagement.Data;
 using InventoryManagement.DTOs.User;
 using InventoryManagement.DTOs.Role;
 using InventoryManagement.DTOs;
using InventoryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.DTOs.Auth;
namespace InventoryManagement.Services;
 public class AuthService : IAuthService
 {
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IOtpService _otpService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITokenService _tokenService;

    public AuthService(AppDbContext context,
     IPasswordHasher<User> passwordHasher,
      IOtpService otpService,
       IHttpContextAccessor httpContextAccessor,
        ITokenService tokenService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _otpService = otpService;
        _httpContextAccessor = httpContextAccessor;
        _tokenService = tokenService;

    }

    public async Task<UserDto> RegisterAsync(RegisterDto dto)
{
    if (await _context.Users.AnyAsync(u =>
        u.Email == dto.Email || u.UserName == dto.UserName))
        throw new Exception("User already exists");

    var customerRole = await _context.Roles
        .FirstOrDefaultAsync(r => r.Name == "Customer");

    if (customerRole == null)
        throw new Exception("Customer role not found");

    var user = new User
    {
        FirstName = dto.FirstName,
        LastName = dto.LastName,
        UserName = dto.UserName,
        PhoneNumber = dto.PhoneNumber,
        Email = dto.Email,
        DateOfBirth = dto.DateOfBirth,
        Gender = dto.Gender,
        Address = dto.Address,
        IsActive = false,
        EmailVerified = false,

        UserRoles = new List<UserRole>
        {
            new UserRole
            {
                RoleId = customerRole.Id
            }
        }
    };

    user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

    // STEP 1: Save user first (so we get user.Id)
    _context.Users.Add(user);
    await _context.SaveChangesAsync();

    // STEP 2: Create wallet for user
    var wallet = new Wallet
    {
        CustomerId = user.Id,
        Balance = 0,
        CreatedAt = DateTime.UtcNow
    };

    _context.Wallets.Add(wallet);
    await _context.SaveChangesAsync();

    // STEP 3: Load roles (for response)
    await _context.Entry(user)
        .Collection(u => u.UserRoles)
        .Query()
        .Include(ur => ur.Role)
        .LoadAsync();

    // STEP 4: Send OTP
    await _otpService.GenerateOtpAsync(user.Email);

    // STEP 5: Return DTO
    return new UserDto
    {
        Id = user.Id,
        UserName = user.UserName,
        Roles = user.UserRoles
            .Select(ur => new RoleMinDto
            {
                Id = ur.Role.Id,
                Name = ur.Role.Name
            })
            .ToList()
    };
}
 public async Task<AuthResponseDto> LoginAsync(UserLoginDto dto)
{
    var user = await _context.Users
    .Include(u => u.UserRoles)
    .ThenInclude(ur => ur.Role)
        .ThenInclude(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
    .FirstOrDefaultAsync(u => u.Email == dto.Email);

if (user == null)
    throw new InvalidOperationException("Invalid email or password.");

if (!user.EmailVerified)
    throw new InvalidOperationException("Please verify your email before logging in.");

if (!user.IsActive)
    throw new InvalidOperationException("Account is deactivated");

var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, dto.Password);
if (result == PasswordVerificationResult.Failed)
    throw new InvalidOperationException("Invalid email or password.");

    var token = _tokenService.GenerateJwtToken(user);

return new AuthResponseDto
{
    User = new UserDto
    {
        Id = user.Id,
        UserName = user.UserName,
        Roles = user.UserRoles
            .Select(ur => new RoleMinDto
            {
                Id = ur.Role.Id,
                Name = ur.Role.Name
            })
            .ToList()
    },
    Token = token
};
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