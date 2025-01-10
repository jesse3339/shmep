using Moq.Protected;
using SHMedicalEquipmentProcessor.Infrastructure.Services;

namespace SHMedicalEquipmentProcessor.Tests;
using Moq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Xunit;

public class MedicalAlertClientTests
{
    
    private readonly Mock<ILogger<MedicalEquipmentAlertClient>> _loggerMock;
    private readonly Mock<HttpMessageHandler> _mockHandler;
    private readonly HttpClient _httpClient;

    public MedicalAlertClientTests()
    {
        _loggerMock = new Mock<ILogger<MedicalEquipmentAlertClient>>();
        _mockHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHandler.Object);
        _httpClient.BaseAddress = new Uri("http://localhost");
    }
    
    [Fact]
    public async Task SendExternalAlertAsync_ShouldReturnTrue_WhenRequestIsSuccessful()
    {
        // Arrange
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
        
        var alertClient = new MedicalEquipmentAlertClient(_httpClient, _loggerMock.Object);

        // Act
        var result = await alertClient.SendExternalAlertAsync(Guid.NewGuid().ToString(), "Medical Equipment", 3);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task SendExternalAlertAsync_ShouldReturnFalse_WhenRequestFails()
    {
        // Arrange
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));
        
        var alertClient = new MedicalEquipmentAlertClient(_httpClient, _loggerMock.Object);

        // Act
        var result = await alertClient.SendExternalAlertAsync(Guid.NewGuid().ToString(), "Medical Equipment", 3);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SendExternalAlertAsync_ShouldIncludeCorrectMessageInRequest()
    {
        // Arrange
        _mockHandler.Protected()
                   .Setup<Task<HttpResponseMessage>>(
                       "SendAsync", 
                       ItExpr.IsAny<HttpRequestMessage>(), 
                       ItExpr.IsAny<CancellationToken>())
                   .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var alertClient = new MedicalEquipmentAlertClient(_httpClient, _loggerMock.Object);
        string orderId = Guid.NewGuid().ToString();
        string description = "Medical Equipment";
        int deliveryNotificationCount = 3;

        // Act
        var result = await alertClient.SendExternalAlertAsync(orderId, description, deliveryNotificationCount);

        // Assert
        Assert.True(result);
        _mockHandler.Protected()
                   .Verify("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>(req =>
                       req.Method == HttpMethod.Post &&
                       req.RequestUri.AbsolutePath == "/alerts" &&
                       req.Content.ReadAsStringAsync().Result.Contains($"Alert for delivered item: Order {orderId}, Item: {description}, Delivery Notifications: {deliveryNotificationCount}")
                       ), ItExpr.IsAny<CancellationToken>()
                   );
    }
}