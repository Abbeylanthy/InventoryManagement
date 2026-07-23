using AutoMapper;                                       
using InventoryManagement.Data;                          
using InventoryManagement.Entities;                      
using InventoryManagement.DTOs.Category;                 
using InventoryManagement.Services.Interfaces;           
using InventoryManagement.Repositories.Interfaces;
using System.IO.Pipelines; 
using InventoryManagement.DTOs.Common;   

namespace InventoryManagement.Services;
public class CategoryService : ICategoryService
{
    private readonly IGenericRepository<Category> _repository;
    private readonly IMapper _mapper;
    

    public CategoryService(IGenericRepository<Category> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    public async Task<CategoryDto> CreateAsync(CategoryCreateDto dto)
    {
        var exists = await _repository.GetAllAsync();
        if (exists.Any(c => c.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("Category with this name already exists.");

        var category = _mapper.Map<Category>(dto);
        await _repository.AddAsync(category);
        await _repository.SaveChangesAsync();
        return _mapper.Map<CategoryDto>(category);
    }

    
    public async Task<PaginatedResponse<CategoryDto>> GetAllAsync(
    string? search = null,
    bool? isActive = null,
    int pageNumber = 1,
    int pageSize = 10)
{
    var categories = await _repository.GetAllAsync();
    var query = categories.AsQueryable();

    if (!string.IsNullOrWhiteSpace(search))
    {
        query = query.Where(c =>
            c.Name.Contains(search,
                StringComparison.OrdinalIgnoreCase));
    }

    if (isActive.HasValue)
    {
        query = query.Where(c => c.IsActive == isActive.Value);
    }

    query = query.OrderByDescending(c => c.CreatedAt);

// Count BEFORE pagination
var totalCount = query.Count();

// Apply pagination ONCE
var pagedCategories = query
    .Skip((pageNumber - 1) * pageSize)
    .Take(pageSize)
    .ToList();

var categoryDtos = _mapper.Map<IEnumerable<CategoryDto>>(pagedCategories);

return new PaginatedResponse<CategoryDto>
{
    Items = categoryDtos,
    PageNumber = pageNumber,
    PageSize = pageSize,
    TotalCount = totalCount,
    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
};
}

    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        var category = await _repository.GetByIdAsync(id);
        return category == null ? null : _mapper.Map<CategoryDto>(category);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleActiveAsync(int id, bool isActive)
    {
        var category = await _repository.GetByIdAsync(id); 
        if (category == null) return false; 
        category.IsActive = isActive;
        category.UpdatedAt = DateTime.UtcNow; 
        await _repository.UpdateAsync(category); 
        await _repository.SaveChangesAsync();

        return true;
    }
public async Task<CategoryDto?> UpdateAsync(int id, CategoryUpdateDto dto)
{
    var category = await _repository.GetByIdAsync(id);
    
    if (category == null) 
        return null;
    category.Name = dto.Name;
    category.Description = dto.Description;
    category.UpdatedAt = DateTime.UtcNow;
    await _repository.UpdateAsync(category);
    await _repository.SaveChangesAsync();
    return _mapper.Map<CategoryDto>(category);
    }

    public async Task<IEnumerable<CategoryDto>> GetDropdownAsync()
{
    var categories = await _repository.GetAllAsync();

    return _mapper.Map<IEnumerable<CategoryDto>>(
        categories.Where(c => c.IsActive)
                  .OrderBy(c => c.Name)
    );
}
}