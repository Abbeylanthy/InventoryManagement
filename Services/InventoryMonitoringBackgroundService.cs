using InventoryManagement.Entities;
using InventoryManagement.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace InventoryManagement.Services;

public class InventoryMonitoringBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<InventoryMonitoringBackgroundService> _logger;
    private readonly int _checkIntervalMinutes;

    public InventoryMonitoringBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<InventoryMonitoringBackgroundService> logger,
        IOptions<InventoryMonitoringSettings> settings)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _checkIntervalMinutes = Math.Max(1, settings.Value.CheckIntervalMinutes);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Inventory monitoring background service started. Interval: {IntervalMinutes} minute(s).", _checkIntervalMinutes); // Log the configured interval

        while (!stoppingToken.IsCancellationRequested) // Loop until cancellation is requested
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var inventoryMonitoringService = scope.ServiceProvider.GetRequiredService<IInventoryMonitoringService>();

                await inventoryMonitoringService.CheckLowStockProductsAsync();
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking low stock products.");
            }

            await Task.Delay(TimeSpan.FromMinutes(_checkIntervalMinutes), stoppingToken); 
        }

        _logger.LogInformation("Inventory monitoring background service stopped.");
    }
}
