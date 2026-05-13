namespace InventoryManagement.Services.Interfaces;

public interface IInventoryMonitoringService
{
       Task CheckLowStockProductsAsync();
}