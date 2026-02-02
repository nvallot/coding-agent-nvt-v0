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
            Email = "john.doe@lucy.sbm.com",
            DisplayName = "John Doe"
        };

        _mockLucyService
            .Setup(x => x.GetUserByIdAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(lucyResponse);

        _mockDataverseService
            .Setup(x => x.UpsertStagedPurchaseOrderAsync(It.IsAny<DataverseStagingPurchaseOrder>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var messageBody = JsonSerializer.Serialize(poMessage);
        var message = ServiceBusModelFactory.ServiceBusReceivedMessage(
            body: BinaryData.FromString(messageBody),
            messageId: "test-message-id",
            correlationId: "test-correlation-id");

        var mockMessageActions = new Mock<ServiceBusMessageActions>();
        mockMessageActions
            .Setup(x => x.CompleteMessageAsync(It.IsAny<ServiceBusReceivedMessage>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _function.Run(message, mockMessageActions.Object);

        // Assert
        _mockLucyService.Verify(
            x => x.GetUserByIdAsync(poMessage.PkmGuid, "test-correlation-id"),
            Times.Once);

        _mockDataverseService.Verify(
            x => x.UpsertStagedPurchaseOrderAsync(It.IsAny<DataverseStagingPurchaseOrder>(), "test-correlation-id"),
            Times.Once);

        mockMessageActions.Verify(
            x => x.CompleteMessageAsync(message, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Run_WhenPkmNotFound_ShouldDeadLetterMessage()
    {
        // Arrange
        var poMessage = new PurchaseOrderMessage
        {
            PoNumber = "PO-12345",
            MdmNumber = "MDM-67890",
            PkmGuid = "invalid-pkm-guid",
            PkmEmail = "john.doe@sbm.com",
            Amount = 250000
        };

        _mockLucyService
            .Setup(x => x.GetUserByIdAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((LucyUserResponse?)null);

        var messageBody = JsonSerializer.Serialize(poMessage);
        var message = ServiceBusModelFactory.ServiceBusReceivedMessage(
            body: BinaryData.FromString(messageBody),
            messageId: "test-message-id",
            correlationId: "test-correlation-id");

        var mockMessageActions = new Mock<ServiceBusMessageActions>();
        mockMessageActions
            .Setup(x => x.DeadLetterMessageAsync(
                It.IsAny<ServiceBusReceivedMessage>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _function.Run(message, mockMessageActions.Object);

        // Assert
        mockMessageActions.Verify(
            x => x.DeadLetterMessageAsync(
                message,
                "PKMNotFound",
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _mockDataverseService.Verify(
            x => x.UpsertStagedPurchaseOrderAsync(It.IsAny<DataverseStagingPurchaseOrder>(), It.IsAny<string>()),
            Times.Never);
    }
}
