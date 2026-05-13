using Microsoft.EntityFrameworkCore;
using InventoryManagement.DTOs.User;
using InventoryManagement.DTOs.Role;
using InventoryManagement.Entities;
using InventoryManagement.Data;
using InventoryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace InventoryManagement.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IOtpService _otpService;

    public UserService(
        AppDbContext context,
        IPasswordHasher<User> passwordHasher,
        IOtpService otpService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _otpService = otpService;
    }

    // ---------------- CREATE USER ----------------
    public async Task<UserDto> CreateUserAsync(UserCreateDto dto)
    {
        if (await _context.Users.AnyAsync(u =>
            u.UserName == dto.UserName || u.Email == dto.Email))
        {
            throw new Exception("User already exists");
        }

        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            UserName = dto.UserName,
            Email = dto.Email,
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            IsActive = false,
            EmailVerified = false,

           UserRoles = dto.RoleIds.Select(roleId => new UserRole 
{
    RoleId = roleId 
}).ToList() 
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        await _otpService.GenerateOtpAsync(user.Email);

        await _context.Entry(user)
            .Collection(u => u.UserRoles)
            .Query()
            .Include(r => r.Role)
            .LoadAsync();

        return new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Roles = user.UserRoles
    .Select(r => new RoleMinDto
    {
        Id = r.Role.Id,
        Name = r.Role.Name
    })
    .ToList()
    };
    }

    // ---------------- GET ALL ----------------
    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _context.Users 
            .Include(u => u.UserRoles)
            .ThenInclude(r => r.Role)
            .ToListAsync();

        return users.Select(u => new UserDto
        {
            Id = u.Id,
            UserName = u.UserName,
             Roles = u.UserRoles
    .Select(r => new RoleMinDto
    {
        Id = r.Role.Id,
        Name = r.Role.Name
    })
    .ToList()
        });
    }

    // ---------------- GET BY ID ----------------
    public async Task<UserDto> GetByIdAsync(int id)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(r => r.Role)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null) return null!;

        return new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
             Roles = user.UserRoles
    .Select(r => new RoleMinDto
    {
        Id = r.Role.Id,
        Name = r.Role.Name
    })
    .ToList()
        };
    }

    // ---------------- GET BY ROLE ----------------
    public async Task<IEnumerable<UserDto>> GetByRoleIdAsync(int roleId)
    {
        var users = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(r => r.Role)
            .Where(u => u.UserRoles.Any(r => r.RoleId == roleId))
            .ToListAsync();

        return users.Select(u => new UserDto
        {
            Id = u.Id,
            UserName = u.UserName,
             Roles = u.UserRoles
    .Select(r => new RoleMinDto
    {
        Id = r.Role.Id,
        Name = r.Role.Name
    })
    .ToList()
        });
    }

    // ---------------- GET BY GENDER ----------------
    public async Task<IEnumerable<UserDto>> GetByGenderAsync(string gender)
    {
        var users = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(r => r.Role)
            .Where(u => u.Gender == gender)
            .ToListAsync();

        return users.Select(u => new UserDto
        {
            Id = u.Id,
            UserName = u.UserName,
             Roles = u.UserRoles
    .Select(r => new RoleMinDto
    {
        Id = r.Role.Id,
        Name = r.Role.Name
    })
    .ToList()
        });
    }

    // ---------------- UPDATE USER ----------------
    public async Task<UserDto> UpdateUserAsync(int id, UserUpdateDto dto)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(r => r.Role)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            throw new Exception("User not found");

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.UserName = dto.UserName;
        user.Email = dto.Email;
        user.DateOfBirth = dto.DateOfBirth;
        user.Gender = dto.Gender;

        await _context.SaveChangesAsync();

        return new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
             Roles = user.UserRoles
    .Select(r => new RoleMinDto
    {
        Id = r.Role.Id,
        Name = r.Role.Name
    })
    .ToList()
        };
    }

    // ---------------- DELETE ----------------
    public async Task<bool> DeleteUserAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
            return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    // ---------------- DEACTIVATE ----------------
    public async Task<bool> DeactivateAccountAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
            return false;

        user.IsActive = false;
        await _context.SaveChangesAsync();

        return true;
    }
}