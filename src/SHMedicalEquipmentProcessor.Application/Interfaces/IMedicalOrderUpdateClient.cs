using System.Text.Json;

namespace SHMedicalEquipmentProcessor.Application.Interfaces;

public interface IMedicalOrderUpdateClient
{
    public Task UpdateOrderAsync(JsonDocument order);
}