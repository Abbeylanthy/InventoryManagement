using InventoryManagement.DTOs.User;

namespace InventoryManagement.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateJwtToken(UserDto user);
    }
}