using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using SHMedicalEquipmentProcessor.Domain.Entities;
using SHMedicalEquipmentProcessor.Infrastructure.Services;

namespace SHMedicalEquipmentProcessor.Tests;

public class MedicalOrderClientTests
{
    private readonly Mock<ILogger<MedicalEquipmentOrderClient>> _loggerMock;
    private readonly Mock<HttpMessageHandler> _mockHandler;
    private readonly HttpClient _httpClient;

    public MedicalOrderClientTests()
    {
        _loggerMock = new Mock<ILogger<MedicalEquipmentOrderClient>>();
        _mockHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHandler.Object);
        _httpClient.BaseAddress = new Uri("http://localhost");
    }

    [Fact]
    public async Task GetMedicalEquipmentOrdersAsync_ShouldReturnOrders_WhenResponseIsSuccessful()
    {
        // Arrange
        var orders = new List<Order>
        {
            new Order 
            { 
                OrderId = Guid.NewGuid(),
                Items = new List<Item>
                {
                    new Item { Status = "Delivered", Description = "Item A", DeliveryNotifications = 2 },
                    new Item { Status = "Pending", Description = "Item B", DeliveryNotifications = 1 }
                }
            },
            new Order 
            { 
                OrderId = Guid.NewGuid(),
                Items = new List<Item>
                {
                    new Item { Status = "Delivered", Description = "Item C", DeliveryNotifications = 3 }
                }
            }
        };

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(orders), System.Text.Encoding.UTF8, "application/json")
        };

        _mockHandler.Protected()
                    .Setup<Task<HttpResponseMessage>>(
                        "SendAsync", 
                        ItExpr.IsAny<HttpRequestMessage>(), 
                        ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(response);

        var orderClient = new MedicalEquipmentOrderClient(_httpClient, _loggerMock.Object);

        // Act
        var result = await orderClient.GetMedicalEquipmentOrdersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);  // Ensure two orders are returned
        Assert.Equal(2, result[0].Items.Count);  // Ensure first order has two items
        Assert.Equal("Item A", result[0].Items[0].Description);  // Ensure the item description matches
        Assert.Equal(2, result[0].Items[0].DeliveryNotifications);  // Ensure delivery notifications count
        Assert.True(result[0].Items[0].IsDelivered);  // Ensure the item is delivered
    }

    [Fact]
    public async Task GetMedicalEquipmentOrdersAsync_ShouldReturnNull_WhenResponseFails()
    {
        var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);

        _mockHandler.Protected()
                    .Setup<Task<HttpResponseMessage>>(
                        "SendAsync", 
                        ItExpr.IsAny<HttpRequestMessage>(), 
                        ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(response);

        var orderClient = new MedicalEquipmentOrderClient(_httpClient, _loggerMock.Object);

        // Act
        var result = await orderClient.GetMedicalEquipmentOrdersAsync();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetMedicalEquipmentOrdersAsync_ShouldReturnNull_WhenResponseIsEmpty()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("[]", System.Text.Encoding.UTF8, "application/json") // Empty array
        };

        _mockHandler.Protected()
                    .Setup<Task<HttpResponseMessage>>(
                        "SendAsync", 
                        ItExpr.IsAny<HttpRequestMessage>(), 
                        ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(response);

        var orderClient = new MedicalEquipmentOrderClient(_httpClient, _loggerMock.Object);

        // Act
        var result = await orderClient.GetMedicalEquipmentOrdersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}