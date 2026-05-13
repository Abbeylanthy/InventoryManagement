using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using InventoryManagement.DTOs.User;
using InventoryManagement.Data;
using InventoryManagement.Entities;

namespace InventoryManagement.Services.Interfaces
{

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    private readonly AppDbContext _context;

    public TokenService(IConfiguration config, AppDbContext context)
    {
        _config = config;
        _context = context;
    }

    public string GenerateJwtToken(User user)
{
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.UserName ?? string.Empty)
    };

    foreach (var userRole in user.UserRoles)
    {
        claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
    }

    var key = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
    );

    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: _config["Jwt:Issuer"],
        audience: _config["Jwt:Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddHours(8),
        signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}
}
}