using Microsoft.EntityFrameworkCore;
using InventoryManagement.Data;
using InventoryManagement.Entities;
using InventoryManagement.DTOs.Role;
using InventoryManagement.DTOs.Common;
using InventoryManagement.DTOs.Permission;
using InventoryManagement.Services.Interfaces;

namespace InventoryManagement.Services;

public class RoleService : IRoleService
{
    private readonly AppDbContext _context;

    public RoleService(AppDbContext context)
    {
        _context = context;
    }

    // ---------------- CREATE ROLE ----------------
    public async Task<RoleDto> CreateRoleAsync(CreateRoleDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            throw new InvalidOperationException("Role name is required");
        }
        var exists = await _context.Roles
            .AnyAsync(r => r.Name == dto.Name);

        if (exists)
            throw new Exception("Role already exists");

        var role = new Role
        {
            Name = dto.Name,
            IsActive = true
        };

        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            IsActive = role.IsActive
        };
    }

   public async Task<PaginatedResponse<RoleDto>> GetAllRolesAsync(
    string? search = null,
    bool? isActive = null,
    int pageNumber = 1,
    int pageSize = 10)
{
    var query = _context.Roles.AsQueryable();

    // Active filter
    if (isActive.HasValue)
    {
        query = query.Where(r => r.IsActive == isActive.Value);
    }

    // Search
    if (!string.IsNullOrWhiteSpace(search))
    {
        query = query.Where(r =>
            r.Name.Contains(search));
    }
    var totalCount = await query.CountAsync();
    var roles = await query
        .OrderByDescending(r => r.Id)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    var items = roles.Select(r => new RoleDto
{
    Id = r.Id,
    Name = r.Name,
    IsActive = r.IsActive
}).ToList();

return new PaginatedResponse<RoleDto>
{
    Items = items,
    PageNumber = pageNumber,
    PageSize = pageSize,
    TotalCount = totalCount,
    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
};
}

    // ---------------- GET ROLE BY ID ----------------
   public async Task<RoleDetailsDto?> GetRoleByIdAsync(int id)
{
    var role = await _context.Roles
        .Include(r => r.RolePermissions)
        .ThenInclude(rp => rp.Permission)
        .FirstOrDefaultAsync(r => r.Id == id);

    if (role == null)
        return null;

    return new RoleDetailsDto
    {
        Id = role.Id,
        Name = role.Name,
        IsActive = role.IsActive,

        Permissions = role.RolePermissions
            .Select(rp => new PermissionMinDto
            {
                Id = rp.Permission.Id,
                Name = rp.Permission.Name
            })
            .ToList()
    };
}

    // ---------------- GET ROLE BY NAME ----------------
    public async Task<RoleDto?> GetRoleByNameAsync(string name)
    {
        var role = await _context.Roles
            .FirstOrDefaultAsync(r => r.Name == name);

        if (role == null) return null;

        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            IsActive = role.IsActive
        };
    }

    // ---------------- UPDATE ROLE ----------------
    public async Task<bool> UpdateRoleAsync(int id, CreateRoleDto dto)
    {
        var role = await _context.Roles.FindAsync(id);

        if (role == null)
            return false;

        role.Name = dto.Name;

        await _context.SaveChangesAsync();
        return true;
    }

 public async Task<bool> ToggleRoleStatusAsync(int id)
{
    var role = await _context.Roles.FindAsync(id);

    if (role == null)
        return false;

    // Prevent deactivating Super Admin
    if (role.IsActive && role.Name == "SuperAdmin")
    {
        throw new InvalidOperationException(
            "The Super Admin role cannot be deactivated."
        );
    }

    // Prevent deactivating roles assigned to users
    if (role.IsActive)
    {
        var assignedToUsers = await _context.UserRoles
            .AnyAsync(ur => ur.RoleId == id);

        if (assignedToUsers)
        {
            throw new InvalidOperationException(
                "This role is assigned to one or more users and cannot be deactivated."
            );
        }
    }

    role.IsActive = !role.IsActive;

    await _context.SaveChangesAsync();

    return true;
}

   public async Task<RoleAssignmentResultDto> AssignRoleToUsersAsync(AssignRoleDto dto)
{
    var role = await _context.Roles.FindAsync(dto.RoleId);

    if (role == null || !role.IsActive)
    {
        return new RoleAssignmentResultDto
        {
            Success = false,
            Message = "Role not found or is inactive."
        };
    }

    var users = await _context.Users
        .Include(u => u.UserRoles)
        .Where(u => dto.UserIds.Contains(u.Id))
        .ToListAsync();

    var assignedUsers = new List<string>();
    var skippedUsers = new List<string>();

    foreach (var user in users)
    {
        var exists = user.UserRoles
            .Any(r => r.RoleId == dto.RoleId);

        if (!exists)
        {
            user.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = dto.RoleId
            });

            assignedUsers.Add(user.UserName);
        }
        else
        {
            skippedUsers.Add(user.UserName);
        }
    }

    await _context.SaveChangesAsync();

    string message;

    if (assignedUsers.Any() && skippedUsers.Any())
    {
        message =
            $"Role assigned to {assignedUsers.Count} user(s). " +
            $"{skippedUsers.Count} user(s) already have this role.";
    }
    else if (assignedUsers.Any())
    {
        message =
            $"Role assigned successfully to {assignedUsers.Count} user(s).";
    }
    else
    {
        message =
            "All selected users already have this role.";
    }

    return new RoleAssignmentResultDto
    {
        Success = true,
        ProcessedUsers = assignedUsers,
        SkippedUsers = skippedUsers,
        Message = message
    };
}

    // ---------------- REMOVE ROLE FROM USER ----------------
   public async Task<RoleAssignmentResultDto> RemoveRoleFromUsersAsync(AssignRoleDto dto)
{
    var users = await _context.Users
        .Include(u => u.UserRoles)
        .Where(u => dto.UserIds.Contains(u.Id))
        .ToListAsync();

    var removedUsers = new List<string>();
    var skippedUsers = new List<string>();

    foreach (var user in users)
    {
        var userRole = user.UserRoles
            .FirstOrDefault(r => r.RoleId == dto.RoleId);

        if (userRole != null)
        {
            user.UserRoles.Remove(userRole);
            removedUsers.Add(user.UserName);
        }
        else
        {
            skippedUsers.Add(user.UserName);
        }
    }

    await _context.SaveChangesAsync();

    string message;

    if (removedUsers.Any() && skippedUsers.Any())
    {
        message =
            $"Role removed from {removedUsers.Count} user(s). " +
            $"{skippedUsers.Count} user(s) already did not have this role.";
    }
    else if (removedUsers.Any())
    {
        message =
            $"Role removed successfully from {removedUsers.Count} user(s).";
    }
    else
    {
        message =
            "None of the selected users currently have this role.";
    }

    return new RoleAssignmentResultDto
    {
        Success = true,
        ProcessedUsers = removedUsers,
        SkippedUsers = skippedUsers,
        Message = message
    };
}
}