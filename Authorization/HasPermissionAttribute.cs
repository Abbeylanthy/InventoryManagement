using Microsoft.AspNetCore.Authorization;

namespace InventoryManagement.Authorization;

public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission) 
    {
        Policy = permission; 
    }
}