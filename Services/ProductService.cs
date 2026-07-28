using Microsoft.EntityFrameworkCore; 
using System.Linq; 
using InventoryManagement.DTOs.Category;                
using InventoryManagement.Data;                     
using AutoMapper;                                    
using InventoryManagement.Entities;                    
using InventoryManagement.DTOs.Product;                
using InventoryManagement.Services.Interfaces;       
using InventoryManagement.Repositories.Interfaces;     
using System.Linq.Expressions;  
using System.Security.Claims; 
using InventoryManagement.DTOs.Common;                  

namespace InventoryManagement.Services;
public class ProductService : IProductService
{
    private readonly IGenericRepository<Product> _repository;
    private readonly IMapper _mapper;
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public ProductService(
        IGenericRepository<Product> repository, 
        IMapper mapper,
        AppDbContext context,
        IHttpContextAccessor httpContextAccessor)
    {
        _repository = repository;
        _mapper = mapper;
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<ProductDto> CreateAsync(ProductCreateDto dto)
    {
        Console.WriteLine(dto.Threshold);
        var product = _mapper.Map<Product>(dto);
        await _repository.AddAsync(product);

        await _repository.SaveChangesAsync();
        var createdDto = await _context.Products
    .Include(p => p.Category)
    .Where(p => p.Id == product.Id)
    .Select(p => new ProductDto
    {
        Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        Price = p.Price,
        Quantity = p.Quantity,
        SKU = p.SKU,
        CategoryId = (int)p.CategoryId,
        Category = p.Category!.Name,
        SupplierId = p.SupplierId,
        Threshold = p.Threshold,
        IsActive = p.IsActive
            
    })
    .FirstOrDefaultAsync();

return createdDto!;
    }
    
public async Task<PaginatedResponse<ProductDto>> GetAllAsync(
    string? search = null,
    string? sort = null,
    bool? isActive = null,
    int pageNumber = 1,
    int pageSize = 10)
{
    IQueryable<Product> query = _context.Products
    .Include(p => p.Category)
    .Include(p => p.Supplier);

    var user = _httpContextAccessor.HttpContext?.User;

bool isAdmin =
    user != null &&
    (
        user.IsInRole("SuperAdmin") ||
        user.IsInRole("Admin") ||
        user.IsInRole("Staff")
    );

    // Search
    if (!string.IsNullOrWhiteSpace(search))
    {
        query = query.Where(p =>
            p.Name.Contains(search) ||
            (p.Description != null && p.Description.Contains(search)));
    }

   // Active filter
if (isAdmin)
{
    if (isActive.HasValue)
    {
        query = query.Where(p => p.IsActive == isActive.Value);
    }
}
else
{
    // Customers always see only active products
    query = query.Where(p => p.IsActive);
}

    // Sorting
    if (!string.IsNullOrWhiteSpace(sort))
    {
        query = sort.ToLower() switch
        {
            "price_asc"  => query.OrderBy(p => p.Price),
            "price_desc" => query.OrderByDescending(p => p.Price),
            "name_asc"   => query.OrderBy(p => p.Name),
            "name_desc"  => query.OrderByDescending(p => p.Name),
            _            => query.OrderByDescending(p => p.CreatedAt)
        };
    }
    else
    {
        query = query.OrderByDescending(p => p.CreatedAt);
    }

    var totalCount = await query.CountAsync();
    var pagedProducts = await query
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    bool canSeeThreshold =
        user != null &&
        (
            user.IsInRole("SuperAdmin") ||
            user.IsInRole("Manager") ||
            user.IsInRole("Staff")
        );

    var productDtos = _mapper.Map<IEnumerable<ProductDto>>(pagedProducts);

    if (!canSeeThreshold)
    {
        foreach (var product in productDtos)
        {
            product.Threshold = null;
        }
    }

    return new PaginatedResponse<ProductDto>
{
    Items = productDtos,
    PageNumber = pageNumber,
    PageSize = pageSize,
    TotalCount = totalCount,
    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
};
}
 public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
    return null;

var user = _httpContextAccessor.HttpContext?.User;

bool canSeeThreshold =
    user != null &&
    (
        user.IsInRole("SuperAdmin") ||
        user.IsInRole("Manager") ||
        user.IsInRole("Staff")
    );

var productDto = _mapper.Map<ProductDto>(product);

if (!canSeeThreshold)
{
    productDto.Threshold = null;
}
return productDto;
 }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _context.Products
            .Include(p => p.PurchaseOrderItems)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return false;

        if (product.PurchaseOrderItems != null && product.PurchaseOrderItems.Any())
            throw new InvalidOperationException("Product cannot be deleted because it is referenced in purchase orders.");

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleActiveAsync(int id, bool isActive)
    {
        var product = await _repository.GetByIdAsync(id);

        if (product == null) 
            return false;

        product.IsActive = isActive;
        product.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(product);
        await _repository.SaveChangesAsync();

        return true;
    }


    public async Task<ProductDto?> UpdateAsync(int id, ProductUpdateDto dto)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null) 
            return null;

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.Quantity = dto.Quantity;
        product.SKU = dto.SKU;
        product.CategoryId = dto.CategoryId;
        product.SupplierId = dto.SupplierId;
        product.Threshold = dto.Threshold;
        product.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(product);
        await _repository.SaveChangesAsync();

        return _mapper.Map<ProductDto>(product);
    }

    public async Task<List<ProductDropdownDto>> GetDropdownAsync()
{
    return await _context.Products
        .Where(p => p.IsActive)
        .OrderBy(p => p.Name)
        .Select(p => new ProductDropdownDto
        {
            Id = p.Id,
            Name = p.Name
        })
        .ToListAsync();
}
}