using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SHMedicalEquipmentProcessor.Application.Common.Interfaces;
using SHMedicalEquipmentProcessor.Domain.Entities;

namespace SHMedicalEquipmentProcessor.Infrastructure.Services;

public class MedicalOrderUpdateClient(
    HttpClient httpClient, 
    ILogger<MedicalOrderUpdateClient> logger
    ) : IMedicalOrderUpdateClient
{
    public async Task<bool> UpdateOrderAsync(Order order)
    {
        // this would likely be a PUT request rather than a post
        var response = await httpClient.PostAsJsonAsync("/update", order);

        if (response.IsSuccessStatusCode)
        {
            return true;
        }
        logger.LogError("Error while updating order {OrderId}", order.OrderId.ToString());
        return false;
    }
}