using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using SHMedicalEquipmentProcessor.Application.Interfaces;

namespace SHMedicalEquipmentProcessor.Infrastructure.Services;

public class MedicalEquipmentAlertClient(
    HttpClient httpClient,
    ILogger<MedicalEquipmentAlertClient> logger
    ) : IMedicalEquipmentAlertClient
{
    public async Task<bool> SendExternalAlertAsync(string orderId, string description, int deliveryNotificationCount)
    {
        var response = await httpClient.PostAsJsonAsync("/alerts", new
        {
            Message = $"Alert for delivered item: Order {orderId}, Item: {description}, " +
                      $"Delivery Notifications: {deliveryNotificationCount}"
        });

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("Failed to send alert for delivered item: {Description}", description);
            return false;
        }
        
        logger.LogDebug("Alert sent for delivered item: {Description}", description);
        return true;
    }
}