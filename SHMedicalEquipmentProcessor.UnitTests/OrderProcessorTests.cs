using FluentAssertions;
using Moq;
using SHMedicalEquipmentProcessor.Application.Common.Interfaces;
using SHMedicalEquipmentProcessor.Application.Features;
using SHMedicalEquipmentProcessor.Domain.Entities;

namespace SHMedicalEquipmentProcessor.Tests;

public class OrderProcessorTests
{ 
    private readonly Mock<IMedicalEquipmentAlertClient> _alertClientMock;
    private readonly Mock<IMedicalEquipmentOrderClient> _orderClientMock;
    private readonly Mock<IMedicalOrderUpdateClient> _orderUpdateClientMock;
    private readonly OrderProcessor _orderProcessor;

    public OrderProcessorTests()
    {
        _alertClientMock = new Mock<IMedicalEquipmentAlertClient>();
        _orderClientMock = new Mock<IMedicalEquipmentOrderClient>();
        _orderUpdateClientMock = new Mock<IMedicalOrderUpdateClient>();

        _orderProcessor = new OrderProcessor(
            _alertClientMock.Object,
            _orderClientMock.Object,
            _orderUpdateClientMock.Object
        );
    }
    
    [Fact]
    public async Task ProcessOrders_ShouldReturnEarly_WhenNoOrdersToProcess()
    {
        // Arrange
        _orderClientMock.Setup(client => client.GetMedicalEquipmentOrdersAsync())
            .ReturnsAsync(null as List<Order>);  // Simulate no orders

        // Act
        await _orderProcessor.ProcessOrders();

        // Assert
        _alertClientMock.Verify(client => client.SendExternalAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        _orderUpdateClientMock.Verify(client => client.UpdateOrderAsync(It.IsAny<Order>()), Times.Never);
    }

    [Fact]
    public async Task ProcessOrders_ShouldProcessEachOrderItem_WhenOrdersExist()
    {
        // Arrange
        var order = new Order
        {
            OrderId = Guid.NewGuid(),
            Items = new List<Item>
            {
                new Item { Description = "Item 1", Status = "Delivered", DeliveryNotifications = 0 },
                new Item { Description = "Item 2", Status = "Processing", DeliveryNotifications = 0 }
            }
        };
        var orders = new List<Order> { order };

        _orderClientMock.Setup(client => client.GetMedicalEquipmentOrdersAsync()).ReturnsAsync(orders);
        _alertClientMock.Setup(client => client.SendExternalAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(true);  // Simulate successful alert sending

        // Act
        await _orderProcessor.ProcessOrders();

        // Assert
        _alertClientMock.Verify(client => client.SendExternalAlertAsync(order.OrderId.ToString(), "Item 1", 0), Times.Once);
        _orderUpdateClientMock.Verify(client => client.UpdateOrderAsync(order), Times.Once);

        // Assert that delivery notifications for Item 1 were incremented
        order.Items[0].DeliveryNotifications.Should().Be(1);
    }

    [Fact]
    public async Task ProcessOrders_ShouldNotIncrementNotification_WhenAlertFails()
    {
        // Arrange
        var order = new Order
        {
            OrderId = Guid.NewGuid(),
            Items = new List<Item>
            {
                new Item { Description = "Item 1", Status = "Delivered", DeliveryNotifications = 0 }
            }
        };
        var orders = new List<Order> { order };

        _orderClientMock.Setup(client => client.GetMedicalEquipmentOrdersAsync()).ReturnsAsync(orders);
        _alertClientMock.Setup(client => client.SendExternalAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(false);  // Simulate failed alert sending

        // Act
        await _orderProcessor.ProcessOrders();

        // Assert
        _alertClientMock.Verify(client => client.SendExternalAlertAsync(order.OrderId.ToString(), "Item 1", 0), Times.Once);
        _orderUpdateClientMock.Verify(client => client.UpdateOrderAsync(order), Times.Once);

        // Assert that delivery notifications for Item 1 were not incremented
        order.Items[0].DeliveryNotifications.Should().Be(0);
    }

    [Fact]
    public async Task ProcessOrderItems_ShouldNotProcessItemsIfNotDelivered()
    {
        // Arrange
        var order = new Order
        {
            OrderId = Guid.NewGuid(),
            Items = new List<Item>
            {
                new Item { Description = "Item 1", Status = "Processing", DeliveryNotifications = 0 },
                new Item { Description = "Item 2", Status = "Processing", DeliveryNotifications = 0 }
            }
        };

        // Act
        await _orderProcessor.ProcessOrderItems(order);

        // Assert
        _alertClientMock.Verify(client => client.SendExternalAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        order.Items[0].DeliveryNotifications.Should().Be(0);  // No change
        order.Items[1].DeliveryNotifications.Should().Be(0);  // No change
    }
}