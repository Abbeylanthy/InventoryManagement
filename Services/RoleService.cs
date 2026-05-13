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
    public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
    {
        var roles = await _context.Roles.ToListAsync();

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

    // ---------------- DEACTIVATE ROLE ----------------
    public async Task<bool> DeactivateRoleAsync(int id)
    {
        var role = await _context.Roles.FindAsync(id);

        if (role == null)
            return false;

        role.IsActive = false;

        await _context.SaveChangesAsync();
        return true;
    }

    // ---------------- ASSIGN ROLE TO USER (MANY-TO-MANY) ----------------
    public async Task<bool> AssignRoleToUserAsync(int userId, int roleId)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return false;

        var role = await _context.Roles.FindAsync(roleId);

        if (role == null || !role.IsActive)
            return false;

        var exists = user.UserRoles
            .Any(r => r.RoleId == roleId);

        if (exists)
            return true;

        user.UserRoles.Add(new UserRole
        {
            UserId = userId,
            RoleId = roleId
        });

        await _context.SaveChangesAsync();
        return true;
    }

    // ---------------- REMOVE ROLE FROM USER ----------------
    public async Task<bool> RemoveRoleFromUserAsync(int userId, int roleId)
    {
        var userRole = await _context.UserRoles
            .FirstOrDefaultAsync(x => x.UserId == userId && x.RoleId == roleId);

        if (userRole == null)
            return false;

        _context.UserRoles.Remove(userRole);
        await _context.SaveChangesAsync();

        return true;
    }
}