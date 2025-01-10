using SHMedicalEquipmentProcessor.Application.Common.Interfaces;
using SHMedicalEquipmentProcessor.Domain.Entities;

namespace SHMedicalEquipmentProcessor.Application.Features;

public class OrderProcessor(
    IMedicalEquipmentAlertClient alertClient,
    IMedicalEquipmentOrderClient orderClient,
    IMedicalOrderUpdateClient orderUpdateClient
    )
{
    public async Task ProcessOrders()
    {
        
        var orders = await orderClient.GetMedicalEquipmentOrdersAsync();

        if (orders is null or not { Count: > 0 })
        {
            return;
        }
        
        foreach (var order in orders)
        {
            await ProcessOrderItems(order);
            await orderUpdateClient.UpdateOrderAsync(order);
        }
        
    }

    public async Task ProcessOrderItems(Order order)
    {
        foreach (var item in order.Items.Where(item => item.IsDelivered))
        {
            var alertSent = await alertClient.SendExternalAlertAsync(
                order.OrderId.ToString(), 
                item.Description, 
                item.DeliveryNotifications);

            if (alertSent)
            {
                item.IncrementDeliveryNotifications();
            }
        }
    }
}