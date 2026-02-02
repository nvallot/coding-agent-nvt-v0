using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Azure.Messaging.ServiceBus;
using FAP_57.SendPOSupplier;
using FAP_57.SendPOSupplier.Services;
using Shared.Models;
using System.Text;
using System.Text.Json;

namespace FAP_57.Tests;

public class SendPOSupplierFunctionTests
{
    private readonly Mock<ILogger<SendPOSupplierFunction>> _mockLogger;
    private readonly Mock<ILucyApiService> _mockLucyService;
    private readonly Mock<IDataverseService> _mockDataverseService;
    private readonly SendPOSupplierFunction _function;

    public SendPOSupplierFunctionTests()
    {
        _mockLogger = new Mock<ILogger<SendPOSupplierFunction>>();
        _mockLucyService = new Mock<ILucyApiService>();
        _mockDataverseService = new Mock<IDataverseService>();

        _function = new SendPOSupplierFunction(
            _mockLogger.Object,
            _mockLucyService.Object,
            _mockDataverseService.Object);
    }

    [Fact]
    public async Task Run_WithValidMessage_ShouldEnrichAndSendToDataverse()
    {
        // Arrange
        var poMessage = new PurchaseOrderMessage
        {
            PoNumber = "PO-12345",
            MdmNumber = "MDM-67890",
            PkmGuid = "12345678-1234-1234-1234-123456789012",
            PkmEmail = "john.doe@sbm.com",
            Amount = 250000,
            ProductCode = "PKG.123"
        };

        var lucyResponse = new LucyUserResponse
        {
            Id = "12345678-1234-1234-1234-123456789012",
            GivenName = "John",
            SurName = "Doe",
            Email = "john.doe@sbm.com"
        };

        var messageJson = JsonSerializer.Serialize(poMessage);
        var messageBody = BinaryData.FromString(messageJson);
        
        // Mock ServiceBusReceivedMessage behavior
        var mockMessage = ServiceBusModelFactory.ServiceBusReceivedMessage(
            body: messageBody,
            messageId: "msg-123",
            correlationId: "corr-123",
            deliveryCount: 1);

        var mockActions = new Mock<ServiceBusMessageActions>();

        _mockLucyService
            .Setup(x => x.GetUserByIdAsync(poMessage.PkmGuid, It.IsAny<string>()))
            .ReturnsAsync(lucyResponse);

        // Act
        await _function.Run(mockMessage, mockActions.Object);

        // Assert
        _mockLucyService.Verify(
            x => x.GetUserByIdAsync(poMessage.PkmGuid, It.IsAny<string>()),
            Times.Once);

        _mockDataverseService.Verify(
            x => x.UpsertStagedPurchaseOrderAsync(
                It.Is<DataverseStagingPurchaseOrder>(e => 
                    e.PoNumber == "PO-12345" &&
                    e.PkmFirstName == "John" &&
                    e.PkmLastName == "Doe"),
                It.IsAny<string>()),
            Times.Once);

        mockActions.Verify(
            x => x.CompleteMessageAsync(mockMessage, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Run_WhenPKMNotFound_ShouldDeadLetterMessage()
    {
        // Arrange
        var poMessage = new PurchaseOrderMessage
        {
            PoNumber = "PO-12345",
            PkmGuid = "unknown-pkm-guid"
        };

        var messageJson = JsonSerializer.Serialize(poMessage);
        var messageBody = BinaryData.FromString(messageJson);
        
        var mockMessage = ServiceBusModelFactory.ServiceBusReceivedMessage(
            body: messageBody,
            messageId: "msg-123",
            deliveryCount: 1);

        var mockActions = new Mock<ServiceBusMessageActions>();

        _mockLucyService
            .Setup(x => x.GetUserByIdAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((LucyUserResponse?)null);

        // Act
        await _function.Run(mockMessage, mockActions.Object);

        // Assert
        mockActions.Verify(
            x => x.DeadLetterMessageAsync(
                mockMessage,
                It.IsAny<Dictionary<string, object>>(),
                "PKMNotFound",
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _mockDataverseService.Verify(
            x => x.UpsertStagedPurchaseOrderAsync(It.IsAny<DataverseStagingPurchaseOrder>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Run_WithInvalidJson_ShouldDeadLetterMessage()
    {
        // Arrange
        var messageBody = BinaryData.FromString("{ invalid json");
        
        var mockMessage = ServiceBusModelFactory.ServiceBusReceivedMessage(
            body: messageBody,
            messageId: "msg-123",
            deliveryCount: 1);

        var mockActions = new Mock<ServiceBusMessageActions>();

        // Act
        await _function.Run(mockMessage, mockActions.Object);

        // Assert
        mockActions.Verify(
            x => x.DeadLetterMessageAsync(
                mockMessage,
                It.IsAny<Dictionary<string, object>>(),
                "InvalidMessageFormat",
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Run_WhenDataverseFailsAndMaxRetries_ShouldDeadLetter()
    {
        // Arrange
        var poMessage = new PurchaseOrderMessage
        {
            PoNumber = "PO-12345",
            PkmGuid = "12345678-1234-1234-1234-123456789012"
        };

        var lucyResponse = new LucyUserResponse
        {
            GivenName = "John",
            SurName = "Doe"
        };

        var messageJson = JsonSerializer.Serialize(poMessage);
        var messageBody = BinaryData.FromString(messageJson);
        
        var mockMessage = ServiceBusModelFactory.ServiceBusReceivedMessage(
            body: messageBody,
            messageId: "msg-123",
            deliveryCount: 5); // Max delivery count reached

        var mockActions = new Mock<ServiceBusMessageActions>();

        _mockLucyService
            .Setup(x => x.GetUserByIdAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(lucyResponse);

        _mockDataverseService
            .Setup(x => x.UpsertStagedPurchaseOrderAsync(It.IsAny<DataverseStagingPurchaseOrder>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("Dataverse error"));

        // Act
        await _function.Run(mockMessage, mockActions.Object);

        // Assert
        mockActions.Verify(
            x => x.DeadLetterMessageAsync(
                mockMessage,
                It.IsAny<Dictionary<string, object>>(),
                "MaxDeliveryAttemptsExceeded",
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
