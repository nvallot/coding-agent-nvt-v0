using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker;
using FAP_65.RetrivePOVendor;
using FAP_65.RetrivePOVendor.Services;
using Shared.Models;

namespace FAP_65.Tests;

public class RetrivePOVendorFunctionTests
{
    private readonly Mock<ILogger<RetrivePOVendorFunction>> _mockLogger;
    private readonly Mock<INadiaDataService> _mockNadiaService;
    private readonly Mock<IServiceBusPublisher> _mockServiceBusPublisher;
    private readonly Mock<ILastExecutionService> _mockLastExecutionService;
    private readonly RetrivePOVendorFunction _function;

    public RetrivePOVendorFunctionTests()
    {
        _mockLogger = new Mock<ILogger<RetrivePOVendorFunction>>();
        _mockNadiaService = new Mock<INadiaDataService>();
        _mockServiceBusPublisher = new Mock<IServiceBusPublisher>();
        _mockLastExecutionService = new Mock<ILastExecutionService>();

        _function = new RetrivePOVendorFunction(
            _mockLogger.Object,
            _mockNadiaService.Object,
            _mockServiceBusPublisher.Object,
            _mockLastExecutionService.Object);
    }

    [Fact]
    public async Task Run_FirstExecution_ShouldUse30DaysLookback()
    {
        // Arrange
        var timerInfo = new TimerInfo();
        _mockLastExecutionService
            .Setup(x => x.GetLastExecutionAsync())
            .ReturnsAsync(((DateTime?)null, TimeSpan.Zero));

        _mockNadiaService
            .Setup(x => x.GetPurchaseOrdersAsync(It.IsAny<DateTime>(), It.IsAny<TimeSpan>(), It.IsAny<string>()))
            .ReturnsAsync(new List<PurchaseOrderMessage>());

        // Act
        await _function.Run(timerInfo);

        // Assert
        _mockNadiaService.Verify(x => x.GetPurchaseOrdersAsync(
            It.Is<DateTime>(d => d >= DateTime.Today.AddDays(-30)),
            TimeSpan.Zero,
            It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Run_WithPurchaseOrders_ShouldPublishAllToServiceBus()
    {
        // Arrange
        var timerInfo = new TimerInfo();
        var lastExecDate = DateTime.Today.AddDays(-1);
        var lastExecTime = new TimeSpan(4, 0, 0);

        _mockLastExecutionService
            .Setup(x => x.GetLastExecutionAsync())
            .ReturnsAsync((lastExecDate, lastExecTime));

        var purchaseOrders = new List<PurchaseOrderMessage>
        {
            new() { PoNumber = "PO-001", Amount = 150000 },
            new() { PoNumber = "PO-002", Amount = 200000 },
            new() { PoNumber = "PO-003", Amount = 175000 }
        };

        _mockNadiaService
            .Setup(x => x.GetPurchaseOrdersAsync(lastExecDate, lastExecTime, It.IsAny<string>()))
            .ReturnsAsync(purchaseOrders);

        // Act
        await _function.Run(timerInfo);

        // Assert
        _mockServiceBusPublisher.Verify(
            x => x.PublishAsync(It.IsAny<PurchaseOrderMessage>()),
            Times.Exactly(3));

        _mockLastExecutionService.Verify(
            x => x.UpdateLastExecutionAsync(It.IsAny<DateTime>(), It.IsAny<TimeSpan>()),
            Times.Once);
    }

    [Fact]
    public async Task Run_WithNoPurchaseOrders_ShouldNotPublishAny()
    {
        // Arrange
        var timerInfo = new TimerInfo();
        var lastExecDate = DateTime.Today.AddDays(-1);

        _mockLastExecutionService
            .Setup(x => x.GetLastExecutionAsync())
            .ReturnsAsync((lastExecDate, TimeSpan.Zero));

        _mockNadiaService
            .Setup(x => x.GetPurchaseOrdersAsync(It.IsAny<DateTime>(), It.IsAny<TimeSpan>(), It.IsAny<string>()))
            .ReturnsAsync(new List<PurchaseOrderMessage>());

        // Act
        await _function.Run(timerInfo);

        // Assert
        _mockServiceBusPublisher.Verify(
            x => x.PublishAsync(It.IsAny<PurchaseOrderMessage>()),
            Times.Never);

        _mockLastExecutionService.Verify(
            x => x.UpdateLastExecutionAsync(It.IsAny<DateTime>(), It.IsAny<TimeSpan>()),
            Times.Never);
    }

    [Fact]
    public async Task Run_WhenPublishFails_ShouldContinueWithOtherMessages()
    {
        // Arrange
        var timerInfo = new TimerInfo();
        
        _mockLastExecutionService
            .Setup(x => x.GetLastExecutionAsync())
            .ReturnsAsync((DateTime.Today.AddDays(-1), TimeSpan.Zero));

        var purchaseOrders = new List<PurchaseOrderMessage>
        {
            new() { PoNumber = "PO-001", Amount = 150000 },
            new() { PoNumber = "PO-002", Amount = 200000 }, // This will fail
            new() { PoNumber = "PO-003", Amount = 175000 }
        };

        _mockNadiaService
            .Setup(x => x.GetPurchaseOrdersAsync(It.IsAny<DateTime>(), It.IsAny<TimeSpan>(), It.IsAny<string>()))
            .ReturnsAsync(purchaseOrders);

        _mockServiceBusPublisher
            .Setup(x => x.PublishAsync(It.Is<PurchaseOrderMessage>(p => p.PoNumber == "PO-002")))
            .ThrowsAsync(new Exception("Service Bus error"));

        // Act
        await _function.Run(timerInfo);

        // Assert
        _mockServiceBusPublisher.Verify(
            x => x.PublishAsync(It.IsAny<PurchaseOrderMessage>()),
            Times.Exactly(3));

        _mockLastExecutionService.Verify(
            x => x.UpdateLastExecutionAsync(It.IsAny<DateTime>(), It.IsAny<TimeSpan>()),
            Times.Once);
    }
}
