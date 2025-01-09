using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SHMedicalEquipmentProcessor.Application.Interfaces;

namespace SHMedicalEquipmentProcessor.Infrastructure.Services;

public class MedicalEquipmentOrderClient(
    HttpClient httpClient, 
    ILogger<MedicalEquipmentOrderClient> logger
    ) : IMedicalEquipmentOrderClient
{
    public async Task<JsonDocument?> GetMedicalEquipmentOrdersAsync()
    {
        var response = await httpClient.GetAsync("/orders");

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("GetMedicalEquipmentOrders failed with status code {ResponseStatusCode}", response.StatusCode);
            return null;
        }
        
        // could deserialize into a c# object based on openapi spec if available using ReadFromJsonAsync
        var json = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(json);
    }
}