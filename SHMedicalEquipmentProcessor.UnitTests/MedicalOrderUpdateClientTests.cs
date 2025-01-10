using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using SHMedicalEquipmentProcessor.Domain.Entities;
using SHMedicalEquipmentProcessor.Infrastructure.Services;

namespace SHMedicalEquipmentProcessor.Tests;

public class MedicalOrderUpdateClientTests
{
    private readonly Mock<ILogger<MedicalOrderUpdateClient>> _loggerMock;
    private readonly Mock<HttpMessageHandler> _mockHandler;
    private readonly HttpClient _httpClient;

    public MedicalOrderUpdateClientTests()
    {
        _loggerMock = new Mock<ILogger<MedicalOrderUpdateClient>>();
        _mockHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHandler.Object);
        _httpClient.BaseAddress = new Uri("http://localhost");
    }

    [Fact]
    public async Task UpdateAsync_ShouldIncludeCorrectMessageInRequest()
    {
        // Arrange
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
        
        var updateClient = new MedicalOrderUpdateClient(_httpClient, _loggerMock.Object);
        var order = new Order
        {
            OrderId = Guid.NewGuid(),
            Items = [
                new Item
                {
                    Status = "In Progress",
                    Description = "Some description",
                    DeliveryNotifications = 2,
                }
            ]
        };
        
        // Act
        var result = await updateClient.UpdateOrderAsync(order);
        
        // Assert
        Assert.True(result);
        var json = JsonSerializer.Serialize(order, JsonSerializerOptions.Web);
        _mockHandler.Protected()
            .Verify("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Post &&
                req.RequestUri.AbsolutePath == "/update" &&
                req.Content.ReadAsStringAsync().Result.Contains(json)
                ), ItExpr.IsAny<CancellationToken>()
            );
    }
}