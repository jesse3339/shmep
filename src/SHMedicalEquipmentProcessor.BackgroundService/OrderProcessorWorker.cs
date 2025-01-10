using SHMedicalEquipmentProcessor.Application.Features;

namespace SHMedicalEquipmentProcessor.BackgroundService;

public class OrderProcessorWorker(
    ILogger<OrderProcessorWorker> logger,
    OrderProcessor orderProcessor
    ) : Microsoft.Extensions.Hosting.BackgroundService
{
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await orderProcessor.ProcessOrders();
            await Task.Delay(2000, stoppingToken);
        }
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("OrderProcessorWorker starting at: {time}", DateTimeOffset.Now);
        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("OrderProcessorWorker stopping at: {time}", DateTimeOffset.Now);
        return base.StopAsync(cancellationToken);
    }
}