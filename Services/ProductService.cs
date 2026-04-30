using Microsoft.EntityFrameworkCore; 
using System.Linq; 
using InventoryManagement.DTOs.Category;                  // Allows us to use .Include() and FirstOrDefaultAsync for eager loading
using InventoryManagement.Data;                        // Access to AppDbContext to query products with joins
using AutoMapper;                                      // For converting between Product/ProductDto objects
using InventoryManagement.Entities;                    // Access to Product and Category model classes
using InventoryManagement.DTOs.Product;                // ProductCreateDto, ProductUpdateDto, ProductDto
using InventoryManagement.Services.Interfaces;         // IProductService interface contract
using InventoryManagement.Repositories.Interfaces;     // IGenericRepository interface for CRUD ops
using System.Linq.Expressions;                         // Used to build dynamic search filter expressions

namespace InventoryManagement.Services;

/// <summary>
/// Product Service - Contains all business logic for products
/// This layer handles validation, mapping, and complex operations
/// </summary>
public class ProductService : IProductService
{
    // Generic repository instance for product CRUD operations
    private readonly IGenericRepository<Product> _repository;

    // AutoMapper instance for mapping between domain and DTO objects
    private readonly IMapper _mapper;

    // AppDbContext is used for operations where we need Include() for related data
    private readonly AppDbContext _context;

    /// <summary>
    /// Constructor using Dependency Injection
    /// We receive the tools we need from Program.cs
    /// </summary>
    public ProductService(
        IGenericRepository<Product> repository, 
        IMapper mapper,
        AppDbContext context)
    {
        // Save injected dependencies into private fields
        _repository = repository;
        _mapper = mapper;
        _context = context;
    }

    /// <summary>
    /// Creates a new product and returns it with its Category loaded
    /// </summary>
    public async Task<ProductDto> CreateAsync(ProductCreateDto dto)
    {
        // Convert incoming data (DTO) into Product entity (database model)
        var product = _mapper.Map<Product>(dto);

        // Add the product to the database (in memory)
        await _repository.AddAsync(product);

        // Save changes to SQL Server (this assigns the Id)
        await _repository.SaveChangesAsync();

        // Re-fetch the product WITH its related Category data
        // .Include() tells EF Core to join the Categories table
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
        // Add other fields from ProductDto if you have them (CreatedAt, UpdatedAt, IsActive, etc.)
        Category = p.Category!.Name,
            // Description = p.Category.Description,  // if it exists
    })
    .FirstOrDefaultAsync();

return createdDto!;
    }
    /// Gets all products with support for search, sorting, and pagination
/// Gets all products with support for search, sorting, and pagination.
/// Category is included for each product.
public async Task<IEnumerable<ProductDto>> GetAllAsync(
    string? search = null, 
    string? sort = null, 
    int pageNumber = 1, 
    int pageSize = 10)
{
    // Start the query using AppDbContext with Include (same pattern as GetByIdAsync)
    IQueryable<Product> query = _context.Products
        .Include(p => p.Category);

    // Apply search filter if provided
    if (!string.IsNullOrEmpty(search))
    {
        query = query.Where(p => p.Name.Contains(search) ||
                                (p.Description != null && p.Description.Contains(search)));
    }

    // Apply sorting
    if (!string.IsNullOrEmpty(sort))
    {
        query = sort.ToLower() switch
        {
            "price_asc"  => query.OrderBy(p => p.Price),
            "price_desc" => query.OrderByDescending(p => p.Price),
            "name_asc"   => query.OrderBy(p => p.Name),
            "name_desc"  => query.OrderByDescending(p => p.Name),
            _            => query.OrderBy(p => p.Name)
        };
    }
    else
    {
        query = query.OrderBy(p => p.Name);   // Default sort
    }

    // Apply pagination and execute the query
    var pagedProducts = await query
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    // Map to DTOs (same style as GetByIdAsync)
    return _mapper.Map<IEnumerable<ProductDto>>(pagedProducts);
}
    /// <summary>
    /// Get a product by Id with category included, or null if not found
    /// </summary>
    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        // Query using AppDbContext to include related Category object
        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        // Return null if no product, else map to DTO
        return product == null ? null : _mapper.Map<ProductDto>(product);
    }

    /// <summary>
    /// Deletes a product by Id
    /// </summary>
    public async Task<bool> DeleteAsync(int id)
    {
        // Perform delete through generic repository
        await _repository.DeleteAsync(id);

        // Save change to database
        await _repository.SaveChangesAsync();

        // Return success (can expand to false on error in future)
        return true;
    }

    /// <summary>
    /// Sets a product active/inactive
    /// </summary>
    public async Task<bool> ToggleActiveAsync(int id, bool isActive)
    {
        // Find existing product first
        var product = await _repository.GetByIdAsync(id);

        // If product not found, return false to caller
        if (product == null) 
            return false;

        // Update active status and updated timestamp
        product.IsActive = isActive;
        product.UpdatedAt = DateTime.UtcNow;

        // Mark as updated and save
        await _repository.UpdateAsync(product);
        await _repository.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Updates a product by Id with new values from DTO
    /// </summary>
    public async Task<ProductDto?> UpdateAsync(int id, ProductUpdateDto dto)
    {
        // Find the product to update
        var product = await _repository.GetByIdAsync(id);

        // Not found, return null to indicate 404 scenario
        if (product == null) 
            return null;

        // Apply new values
        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.Quantity = dto.Quantity;
        product.SKU = dto.SKU;
        product.CategoryId = dto.CategoryId;
        product.UpdatedAt = DateTime.UtcNow;

        // Persist changes
        await _repository.UpdateAsync(product);
        await _repository.SaveChangesAsync();

        // Respond with updated product as DTO
        return _mapper.Map<ProductDto>(product);
    }
}