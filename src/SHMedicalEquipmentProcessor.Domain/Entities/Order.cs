namespace SHMedicalEquipmentProcessor.Domain.Entities;

public class Order
{
    public Guid OrderId { get; set; }
    public List<Item> Items { get; set; } = [];
}