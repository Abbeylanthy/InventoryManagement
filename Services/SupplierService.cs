using InventoryManagement.DTOs.Supplier;
using InventoryManagement.Entities;
using InventoryManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.Data;

public class SupplierService : ISupplierService
{
    private readonly AppDbContext _context;

    public SupplierService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SupplierDto> CreateSupplierAsync(SupplierCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
{
    throw new Exception("Supplier name is required");
}

if (string.IsNullOrWhiteSpace(dto.ContactEmail))
{
    throw new Exception("Email is required");
}

var existingSupplier = await _context.Suppliers
    .FirstOrDefaultAsync(s => s.Name == dto.Name);

if (existingSupplier != null)
{
    throw new Exception("Supplier already exists");
}
        var supplier = new Supplier
        {
            Name = dto.Name,
            ContactEmail = dto.ContactEmail,
            PhoneNumber = dto.PhoneNumber,
            Address = dto.Address
        };

        await _context.Suppliers.AddAsync(supplier);
        await _context.SaveChangesAsync();

        return new SupplierDto
        {
            Id = supplier.Id,
            Name = supplier.Name,
            ContactEmail = supplier.ContactEmail,
            PhoneNumber = supplier.PhoneNumber,
            Address = supplier.Address
        };
    }

    public async Task<List<SupplierDto>> GetAllSuppliersAsync()
    {
        return await _context.Suppliers
            .Select(s => new SupplierDto
            {
                Id = s.Id,
                Name = s.Name,
                ContactEmail = s.ContactEmail,
                PhoneNumber = s.PhoneNumber,
                Address = s.Address
            })
            .ToListAsync();
    }

    public async Task<SupplierDto> GetSupplierByIdAsync(int id)
    {
        var supplier = await _context.Suppliers.FindAsync(id);

        if (supplier == null) return null!;

        return new SupplierDto
        {
            Id = supplier.Id,
            Name = supplier.Name,
            ContactEmail = supplier.ContactEmail,
            PhoneNumber = supplier.PhoneNumber,
            Address = supplier.Address
        };
    }

    public async Task<SupplierDto> UpdateSupplierAsync(int id, SupplierCreateDto dto)
{
    var supplier = await _context.Suppliers.FindAsync(id);

    if (supplier == null)
        throw new Exception("Supplier not found");

    supplier.Name = dto.Name;
    supplier.ContactEmail = dto.ContactEmail;
    supplier.PhoneNumber = dto.PhoneNumber;
    supplier.Address = dto.Address;

    await _context.SaveChangesAsync();

    return new SupplierDto
    {
        Id = supplier.Id,
        Name = supplier.Name,
        ContactEmail = supplier.ContactEmail,
        PhoneNumber = supplier.PhoneNumber,
        Address = supplier.Address
    };
}

    public async Task<bool> DeleteSupplierAsync(int id)
    {
        var supplier = await _context.Suppliers.FindAsync(id);

        if (supplier == null) return false;

        _context.Suppliers.Remove(supplier);
        await _context.SaveChangesAsync();

        return true;
    }
}