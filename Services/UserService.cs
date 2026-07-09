using Microsoft.EntityFrameworkCore;
using InventoryManagement.DTOs.User;
using InventoryManagement.DTOs.Role;
using InventoryManagement.DTOs.Permission;
using InventoryManagement.Entities;
using InventoryManagement.Data;
using InventoryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using InventoryManagement.DTOs.Common;

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
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            Address = dto.Address,
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

    public async Task<PaginatedResponse<UserDto>> GetAllAsync(
    string? search = null,
    int pageNumber = 1,
    int pageSize = 10)
{
    IQueryable<User> query = _context.Users
        .Include(u => u.UserRoles)
        .ThenInclude(ur => ur.Role);

    if (!string.IsNullOrWhiteSpace(search))
    {
        query = query.Where(u =>
            u.UserName.Contains(search) ||
            u.FirstName.Contains(search) ||
            u.LastName.Contains(search) ||
            u.Email.Contains(search));
    }
    var totalCount = await query.CountAsync();
    var users = await query
        .OrderByDescending(u => u.Id)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

   var items = users.Select(u => new UserDto
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

}).ToList();

return new PaginatedResponse<UserDto>
{
    Items = items,
    PageNumber = pageNumber,
    PageSize = pageSize,
    TotalCount = totalCount,
    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
};
}

   public async Task<UserDto> GetByIdAsync(int id)
{
    var user = await _context.Users
        .Include(u => u.UserRoles)
            .ThenInclude(r => r.Role)
                .ThenInclude(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
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
                Name = r.Role.Name,

                Permissions = r.Role.RolePermissions
                    .Select(rp => new PermissionMinDto
                    {
                        Id = rp.Permission.Id,
                        Name = rp.Permission.Name
                    })
                    .ToList()
            })
            .ToList()
    };
}

    public async Task<IEnumerable<UserDto>> GetByRoleIdAsync(int roleId)
    {
        var users = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(r => r.Role)
            .Where(u => u.UserRoles.Any(r => r.RoleId == roleId))
            .OrderByDescending(u => u.Id)
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

    public async Task<IEnumerable<UserDto>> GetByGenderAsync(string gender)
    {
        var users = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(r => r.Role)
            .Where(u => u.Gender == gender)
            .OrderByDescending(u => u.Id)
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

    public async Task<bool> DeleteUserAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
            return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

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