using System.Text.Json;
using SHMedicalEquipmentProcessor.Domain.Entities;

namespace SHMedicalEquipmentProcessor.Application.Common.Interfaces;

public interface IMedicalOrderUpdateClient
{
    public Task<bool> UpdateOrderAsync(Order order);
}