using InventoryManagement.Entities;
using AutoMapper;
using InventoryManagement.DTOs.Category;
using InventoryManagement.DTOs.Product;
using InventoryManagement.DTOs.User;
using InventoryManagement.DTOs.Permission;

namespace InventoryManagement.Common.Mappings;
/// AutoMapper Profile - This class defines how to map between Entities and DTOs
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
    .ForMember(dest => dest.Category,
        opt => opt.MapFrom(src => src.Category!.Name))
    .ForMember(dest => dest.Supplier,
        opt => opt.MapFrom(src => src.Supplier!.Name))
    .ForMember(dest => dest.SupplierId,
        opt => opt.MapFrom(src => src.SupplierId));// Include basic category info
// Map UserCreateDto → User (for creating)
          CreateMap<User, UserDto>()
    .ForMember(dest => dest.Roles,
        opt => opt.MapFrom(src =>
            src.UserRoles.Select(ur => ur.Role.Name)));
CreateMap<UserCreateDto, User>();

CreateMap<Permission, PermissionDto>();

CreateMap<CreatePermissionDto, Permission>();
    }
}