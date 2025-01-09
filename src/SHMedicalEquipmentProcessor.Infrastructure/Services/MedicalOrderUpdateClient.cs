using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SHMedicalEquipmentProcessor.Application.Interfaces;

namespace SHMedicalEquipmentProcessor.Infrastructure.Services;

public class MedicalOrderUpdateClient(
    HttpClient httpClient, 
    ILogger<MedicalOrderUpdateClient> logger
    ) : IMedicalOrderUpdateClient
{
    public async Task UpdateOrderAsync(JsonDocument order)
    {
        var response = await httpClient.PostAsJsonAsync("/update", order);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("Error while updating order {}", order.RootElement.GetProperty("OrderId").GetString());
        }
    }
}