using InventoryManagement.Entities;
using AutoMapper;
using InventoryManagement.DTOs.Category;
using InventoryManagement.DTOs.Product;
using InventoryManagement.DTOs.User;


namespace InventoryManagement.Common.Mappings;

/// <summary>
/// AutoMapper Profile - This class defines how to map between Entities and DTOs
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Map CategoryCreateDto → Category (for creating)
        CreateMap<CategoryCreateDto, Category>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())           // Don't map Id (database generates it)
            .ForMember(dest => dest.Products, opt => opt.Ignore())     // Ignore navigation property
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())    // Set automatically
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore());    // Default is true

        // Map Category → CategoryDto (for responses)
        CreateMap<Category, CategoryDto>();

        // Map ProductCreateDto → Product (for creating)
        CreateMap<ProductCreateDto, Product>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.Ignore())     // Navigation property - don't map
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore());

        // Map Product → ProductDto (for responses)
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category!.Name)); // Include basic category info
// Map UserCreateDto → User (for creating)
            CreateMap<User, UserDto>()
    .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

CreateMap<UserCreateDto, User>();
    }
}