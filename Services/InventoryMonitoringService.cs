using InventoryManagement.Data;
using InventoryManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Services;
public class  InventoryMonitoringService : IInventoryMonitoringService
{
    private readonly AppDbContext _context;
    private readonly IEmailService _emailService;

    public InventoryMonitoringService(
        AppDbContext context,
        IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

   public async Task CheckLowStockProductsAsync()
{ 
    try
    {
        var lowStockProducts = await _context.Products 
            .Where(p => p.Quantity < p.Threshold)
            .ToListAsync();

        if (!lowStockProducts.Any()) 
            return;

        var admins = await _context.Users 
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Where(u => u.UserRoles.Any(ur => ur.Role.Name == "Admin")) 
            .ToListAsync();

        if (!admins.Any())
            return;

        var message = "The following products are below threshold:\n\n";

        foreach (var product in lowStockProducts) 
        {
            message += $"Product: {product.Name} | Quantity: {product.Quantity} | Threshold: {product.Threshold}\n";
        }

        foreach (var admin in admins)
        {
            await _emailService.SendEmailAsync(
                admin.Email!,
                "Low Stock Alert",
                message
            );
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("LOW STOCK JOB ERROR: " + ex.Message);
        throw;
    }
}
}