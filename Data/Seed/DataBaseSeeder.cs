using InventoryManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
namespace InventoryManagement.Data.Seed;
public class DataBaseSeeder
{
    private readonly IPasswordHasher<User> _passwordHasher;

    public DataBaseSeeder(IPasswordHasher<User> passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }
    public async Task SeedAsync(AppDbContext context)
    {
        var permissionNames = new List<string>
        {
            "ViewCategories",
            "CreateCategory",
            "ToggleCategory",
            "UpdateCategory",
            "DeleteCategory",

            "ViewProducts",
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
            "View Suppliers",

            "CreatePurchaseOrder",
            "ViewPurchaseOrder",
            "ApprovePurchaseOrder",
            "ReceivePurchaseOrder",
            "CancelPurchaseOrder",

            "GetAllOrders",
            "GetPaidOrders",
            "CancelOrder",
            "UpdateOrderStatus",
            "GetDashboardSummary",

            "ViewPayments",

            "ViewWallets",

            "ViewFeedback",

            "ViewNotifications",

            "ManageFeedback",

            "GetAllCarts",
            "GetCartById",

            "ViewInventory",
            "ViewDashboard",

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

        var roles = new List<string>
{
    "SuperAdmin",
    "Admin",
    "Customer",
    "SeniorStaff",
    "JuniorManager",
    "Manager",
    "Staff"
};

foreach (var roleName in roles)
{
    if (!await context.Roles.AnyAsync(r => r.Name == roleName))
    {
        context.Roles.Add(new Role
        {
            Name = roleName
        });
    }
}

await context.SaveChangesAsync();

// Create default SuperAdmin user if one does not exist
if (!await context.Users.AnyAsync(u => u.Email == "admin@inventory.com"))
{
    var superAdminRoleEntity = await context.Roles
        .FirstAsync(r => r.Name == "SuperAdmin");

    var superAdmin = new User
    {
        FirstName = "System",
        LastName = "Administrator",
        UserName = "superadmin",
        PhoneNumber = "00000000000",
        Email = "admin@inventory.com",
        DateOfBirth = new DateOnly(2000, 1, 1),
        Gender = "Male",
        Address = "System",
        IsActive = true,
        EmailVerified = true
    };

    superAdmin.PasswordHash =
        _passwordHasher.HashPassword(superAdmin, "Admin@123");

    superAdmin.UserRoles.Add(new UserRole
    {
        RoleId = superAdminRoleEntity.Id
    });

    context.Users.Add(superAdmin);

    await context.SaveChangesAsync();
}

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
    
