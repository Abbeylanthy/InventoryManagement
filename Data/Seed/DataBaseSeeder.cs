using InventoryManagement.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Data.Seed;
public class DataBaseSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        var permissionNames = new List<string>
        {
            "CreateCategory",
            "ToggleCategory",
            "UpdateCategory",
            "DeleteCategory",

            "CreateProduct",
            "ToggleProduct",
            "UpdateProduct",
            "DeleteProduct",

            "StockIn",
            "StockOut",
            "StockAdjustment",
            "ViewStockHistory",

            "CreateSupplier",
            "GetSuppliers",
            "GetSupplierById",
            "UpdateSupplier",
            "DeleteSupplier",

            "CreatePurchaseOrder",
            "ViewPurchaseOrder",
            "ApprovePurchaseOrder",
            "ReceivePurchaseOrder",
            "CancelPurchaseOrder",

            "GetAllOrders",
            "GetPaidOrders",
            "UpdateOrderStatus",
            "GetDashboardSummary",

            "ViewPayments",

            "ViewWallets",

            "ViewFeedback",

            "ViewNotifications",

            "ManageFeedback",

            "GetAllCarts",
            "GetCartById",

        };

         foreach (var permissionName in permissionNames) 
        {
            var exists = await context.Permissions
                .AnyAsync(p => p.Name == permissionName);

            if (!exists)
            {
                context.Permissions.Add(new Permission 
                {
                    Name = permissionName 
                }); 
            }
        }

        await context.SaveChangesAsync();

        // FIND EXISTING SUPERADMIN ROLE
        var superAdminRole = await context.Roles
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.Name == "SuperAdmin");

        if (superAdminRole == null)
        {
            throw new Exception(
                "SuperAdmin role does not exist in database.");
        }

        // ASSIGN ALL PERMISSIONS TO SUPERADMIN
        var allPermissions = await context.Permissions.ToListAsync(); 

        foreach (var permission in allPermissions) 
        {
            var alreadyAssigned = await context.RolePermissions
                .AnyAsync(rp =>
                    rp.RoleId == superAdminRole.Id &&
                    rp.PermissionId == permission.Id); 

            if (!alreadyAssigned)
            {
                context.RolePermissions.Add(new RolePermission
                {
                    RoleId = superAdminRole.Id,
                    PermissionId = permission.Id 
                });
            }
        }

        await context.SaveChangesAsync();
    }
}
    
