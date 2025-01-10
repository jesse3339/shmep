using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SHMedicalEquipmentProcessor.Application.Common.Interfaces;
using SHMedicalEquipmentProcessor.Domain.Entities;

namespace SHMedicalEquipmentProcessor.Infrastructure.Services;

public class MedicalEquipmentOrderClient(
    HttpClient httpClient, 
    ILogger<MedicalEquipmentOrderClient> logger
    ) : IMedicalEquipmentOrderClient
{
    public async Task<List<Order>?> GetMedicalEquipmentOrdersAsync()
    {
        var response = await httpClient.GetAsync("/orders");

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("GetMedicalEquipmentOrders failed with status code {ResponseStatusCode}", response.StatusCode);
            return null;
        }
        
        var orders = await response.Content.ReadFromJsonAsync<List<Order>>();
        return orders;
    }
}