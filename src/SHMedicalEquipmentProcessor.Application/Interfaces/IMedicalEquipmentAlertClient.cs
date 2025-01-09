namespace SHMedicalEquipmentProcessor.Application.Interfaces;

public interface IMedicalEquipmentAlertClient
{
    public Task<bool> SendExternalAlertAsync(string orderId, string description, int deliveryNotificationCount);
}