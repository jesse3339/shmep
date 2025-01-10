namespace SHMedicalEquipmentProcessor.Domain.Entities;

public class Item
{
    public string Status { get; set; }
    public string Description { get; set; }
    public int DeliveryNotifications { get; set; }

    public void IncrementDeliveryNotifications()
    {
        DeliveryNotifications++;
    }
    
    public bool IsDelivered => Status.Equals("Delivered", StringComparison.InvariantCultureIgnoreCase);
}