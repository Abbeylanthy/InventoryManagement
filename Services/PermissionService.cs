using AutoMapper;
using InventoryManagement.Data;
using InventoryManagement.DTOs.Permission;
using InventoryManagement.Entities;
using InventoryManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.DTOs.Common;

namespace InventoryManagement.Services;

public class PermissionService : IPermissionService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public PermissionService(
        AppDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PermissionDto> CreatePermissionAsync(CreatePermissionDto dto)
    {
        var exists = await _context.Permissions
            .AnyAsync(p => p.Name == dto.Name); 

        if (exists)
        {
            throw new InvalidOperationException(
                "Permission already exists."
            );
        }

        var permission = new Permission
        {
            Name = dto.Name,
            IsActive = true
        };

        _context.Permissions.Add(permission);

        await _context.SaveChangesAsync();

        return _mapper.Map<PermissionDto>(permission); 
    }

   public async Task<PaginatedResponse<PermissionDto>> GetAllPermissionsAsync(
    string? search = null,
    bool? isActive = null,
    int pageNumber = 1,
    int pageSize = 10)
{
    var query = _context.Permissions.AsQueryable();

    // Filter by active/inactive
    if (isActive.HasValue)
    {
        query = query.Where(p => p.IsActive == isActive.Value);
    }

    // Search
    if (!string.IsNullOrWhiteSpace(search))
    {
        query = query.Where(p =>
            p.Name.Contains(search) ||
            (p.Name != null && p.Name.Contains(search)));
    }
    var totalCount = await query.CountAsync();
    var permissions = await query
        .OrderByDescending(p => p.Id)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    var items = permissions.Select(p => new PermissionDto
{
    Id = p.Id,
    Name = p.Name,
    IsActive = p.IsActive
}).ToList();

return new PaginatedResponse<PermissionDto>
{
    Items = items,
    PageNumber = pageNumber,
    PageSize = pageSize,
    TotalCount = totalCount,
    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
};
}

    public async Task<PermissionDto?> GetPermissionByIdAsync(int id)
    {
        var permission = await _context.Permissions
            .FirstOrDefaultAsync(p => p.Id == id);

        return permission == null 
            ? null 
            : _mapper.Map<PermissionDto>(permission); 
    }

    public async Task<bool> ActivatePermissionAsync(int id)
{
    var permission = await _context.Permissions.FindAsync(id);

    if (permission == null)
        return false;

    permission.IsActive = true;

    await _context.SaveChangesAsync();

    return true;
}

   public async Task<bool> DeactivatePermissionAsync(int id)
{
    var permission = await _context.Permissions
        .FirstOrDefaultAsync(p => p.Id == id);

    if (permission == null)
        return false;

    var isAssignedToAnyRole = await _context.RolePermissions
        .AnyAsync(rp => rp.PermissionId == id);

    if (isAssignedToAnyRole)
        throw new InvalidOperationException(
            "Permission is still assigned to one or more roles");

    permission.IsActive = false;

    await _context.SaveChangesAsync();

    return true;
}

    public async Task<bool> UpdatePermissionAsync(int id, CreatePermissionDto dto)
{
    var permission = await _context.Permissions
        .FirstOrDefaultAsync(p => p.Id == id);

    if (permission == null)
        return false;

    var exists = await _context.Permissions
        .AnyAsync(p => p.Name == dto.Name && p.Id != id);

    if (exists)
        throw new InvalidOperationException("Permission already exists.");

    permission.Name = dto.Name;

    await _context.SaveChangesAsync();

    return true;
}

public async Task<bool> AssignPermissionToRolesAsync(AssignPermissionToRolesDto dto)
{
    var permission = await _context.Permissions.FindAsync(dto.PermissionId);

    if (permission == null || !permission.IsActive)
        return false;

    var roles = await _context.Roles
        .Where(r => dto.RoleIds.Contains(r.Id) && r.IsActive)
        .ToListAsync();

    foreach (var role in roles)
    {
        var exists = await _context.RolePermissions
            .AnyAsync(rp => rp.RoleId == role.Id &&
                            rp.PermissionId == dto.PermissionId);

        if (!exists)
        {
            _context.RolePermissions.Add(new RolePermission
            {
                RoleId = role.Id,
                PermissionId = dto.PermissionId
            });
        }
    }

    await _context.SaveChangesAsync();

    return true;
}

public async Task<bool> RemovePermissionFromRolesAsync(RemovePermissionFromRolesDto dto)
{
    var mappings = await _context.RolePermissions
        .Where(rp => rp.PermissionId == dto.PermissionId &&
                     dto.RoleIds.Contains(rp.RoleId))
        .ToListAsync();

    if (!mappings.Any())
        return false;

    _context.RolePermissions.RemoveRange(mappings);

    await _context.SaveChangesAsync();

    return true;
}
}