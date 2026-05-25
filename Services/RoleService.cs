using Microsoft.EntityFrameworkCore;
using InventoryManagement.Data;
using InventoryManagement.Entities;
using InventoryManagement.DTOs.Role;
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

    // ---------------- GET ALL ROLES ----------------
    public async Task<IEnumerable<RoleDto>> GetAllRolesAsync(bool? isActive)
{
    var query = _context.Roles.AsQueryable();

    if (isActive.HasValue)
    {
        query = query.Where(r => r.IsActive == isActive.Value); 
    }

    var roles = await query
        .OrderByDescending(r => r.Id)
        .ToListAsync();

    return roles.Select(r => new RoleDto
    {
        Id = r.Id,
        Name = r.Name,
        IsActive = r.IsActive
    });
}

    // ---------------- GET ROLE BY ID ----------------
    public async Task<RoleDto?> GetRoleByIdAsync(int id)
    {
        var role = await _context.Roles.FindAsync(id);

        if (role == null) return null;

        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            IsActive = role.IsActive
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

    public async Task<bool> ActivateRoleAsync(int id)
{
    var role = await _context.Roles.FindAsync(id);

    if (role == null)
        return false;

    role.IsActive = true;

    await _context.SaveChangesAsync();
    return true;
}

    // ---------------- DEACTIVATE ROLE ----------------
   public async Task<bool> DeactivateRoleAsync(int id)
{
    var role = await _context.Roles.FindAsync(id);

    if (role == null)
        return false;

    var isAssignedToUser = await _context.UserRoles
        .AnyAsync(ur => ur.RoleId == id);

    if (isAssignedToUser)
        throw new InvalidOperationException(
            "Role is still assigned to one or more users"
        );

    role.IsActive = false;

    await _context.SaveChangesAsync();
    return true;
}
    public async Task<bool> AssignRoleToUsersAsync(AssignRoleDto dto)
{
    var role = await _context.Roles.FindAsync(dto.RoleId);

    if (role == null || !role.IsActive)
        return false;

    var users = await _context.Users
        .Include(u => u.UserRoles)
        .Where(u => dto.UserIds.Contains(u.Id))
        .ToListAsync();

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
        }
    }

    await _context.SaveChangesAsync();

    return true;
}

    // ---------------- REMOVE ROLE FROM USER ----------------
    public async Task<bool> RemoveRoleFromUsersAsync(AssignRoleDto dto)
{
    var users = await _context.Users
        .Include(u => u.UserRoles)
        .Where(u => dto.UserIds.Contains(u.Id))
        .ToListAsync();

    foreach (var user in users)
    {
        var userRole = user.UserRoles
            .FirstOrDefault(r => r.RoleId == dto.RoleId);

        if (userRole != null)
        {
            user.UserRoles.Remove(userRole);
        }
    }

    await _context.SaveChangesAsync();

    return true;
}
}