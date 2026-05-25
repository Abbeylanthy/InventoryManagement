using AutoMapper;                                       // AutoMapper for converting between DTOs and entities
using InventoryManagement.Data;                          // AppDbContext type for dependency injection (if needed)
using InventoryManagement.Entities;                      // Category entity definition
using InventoryManagement.DTOs.Category;                 // CategoryCreateDto, CategoryUpdateDto, CategoryDto
using InventoryManagement.Services.Interfaces;           // ICategoryService contract
using InventoryManagement.Repositories.Interfaces;
using System.IO.Pipelines;       // IGenericRepository interface

namespace InventoryManagement.Services;

/// <summary>
/// Category Service - Contains all business logic for categories
/// </summary>
public class CategoryService : ICategoryService
{
    // Generic repository for Category CRUD operations
    private readonly IGenericRepository<Category> _repository;

    // Mapper to convert between Category entities and DTOs
    private readonly IMapper _mapper;

    // Constructor receives dependencies from DI container
    public CategoryService(IGenericRepository<Category> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    /// <summary>
    /// Creates a new category record
    /// </summary>
    public async Task<CategoryDto> CreateAsync(CategoryCreateDto dto)
    {
        // Pseudo-step 1: receive the input (dto contains Name, Description etc.)
        // Pseudo-step 2: convert this simple DTO into a database entity object used by EF Core
        var category = _mapper.Map<Category>(dto);

        // Pseudo-step 3: stage the new category to be inserted into DB
        await _repository.AddAsync(category);

        // Pseudo-step 4: commit the insert to the actual database
        await _repository.SaveChangesAsync();
        
        // Map back to DTO for response
        return _mapper.Map<CategoryDto>(category);
    }

    /// <summary>
    /// Gets all categories from database
    /// </summary>
    public async Task<IEnumerable<CategoryDto>> GetAllAsync()
    {
        // Pseudo-step 1: query database for all Category records
        var categories = await _repository.GetAllAsync();

        // Newest categories first
        var orderedCategories = categories
            .OrderByDescending(c => c.CreatedAt);

        // Pseudo-step 2: convert the raw Category entities into response DTOs
        return _mapper.Map<IEnumerable<CategoryDto>>(orderedCategories);
    }

    /// <summary>
    /// Gets a category by its numeric ID
    /// </summary>
    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        var category = await _repository.GetByIdAsync(id);

        // Pseudo-step 2: if nothing found return null; else return mapped DTO
        return category == null ? null : _mapper.Map<CategoryDto>(category);
    }

    /// <summary>
    /// Deletes a category by ID (hard delete)
    /// </summary>
    public async Task<bool> DeleteAsync(int id)
    {
        // Pseudo-step 1: ask repository to remove record by id
        await _repository.DeleteAsync(id);

        // Pseudo-step 2: save database changes for deletion to take effect
        await _repository.SaveChangesAsync();

        // Pseudo-step 3: return a simple true flag; controller can treat this as success
        return true;
    }

    /// <summary>
    /// Toggles the active state of a category
    /// </summary>
    public async Task<bool> ToggleActiveAsync(int id, bool isActive)
    {
        // Get the existing category entity
        var category = await _repository.GetByIdAsync(id); 

        // If missing, indicate failure
        if (category == null) return false; 

        // Update the IsActive flag and timestamp
        category.IsActive = isActive;
        category.UpdatedAt = DateTime.UtcNow; 

        // Save the changes in repository and database
        await _repository.UpdateAsync(category); 
        await _repository.SaveChangesAsync();

        return true;
    }
        /// <summary>
/// Updates an existing category (full update)
/// </summary>
public async Task<CategoryDto?> UpdateAsync(int id, CategoryUpdateDto dto)
{
    // Step 1: Find the existing category by ID
    var category = await _repository.GetByIdAsync(id);
    
    // Step 2: If category doesn't exist, return null so controller can return NotFound
    if (category == null) 
        return null;

    // Step 3: Update the properties from DTO
    category.Name = dto.Name;
    category.Description = dto.Description;
    
    // Step 4: Update the timestamp
    category.UpdatedAt = DateTime.UtcNow;

    // Step 5: Tell repository to update the entity
    await _repository.UpdateAsync(category);
    
    // Step 6: Save changes to database
    await _repository.SaveChangesAsync();

    // Step 7: Convert updated entity back to DTO and return it
    return _mapper.Map<CategoryDto>(category);

    }
}