using InventoryManagement.DTOs.User;
using InventoryManagement.Entities;

namespace InventoryManagement.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateJwtToken(User user);
    }
}