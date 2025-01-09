using System.Text.Json;
using System.Text.Json.Nodes;

namespace SHMedicalEquipmentProcessor.Application.Interfaces;

public interface IMedicalEquipmentOrderClient
{
    public Task<JsonDocument?> GetMedicalEquipmentOrdersAsync();
}