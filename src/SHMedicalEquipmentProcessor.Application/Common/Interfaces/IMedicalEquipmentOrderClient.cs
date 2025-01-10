using System.Text.Json;
using SHMedicalEquipmentProcessor.Domain.Entities;

namespace SHMedicalEquipmentProcessor.Application.Common.Interfaces;

public interface IMedicalEquipmentOrderClient
{
    public Task<List<Order>?> GetMedicalEquipmentOrdersAsync();
}