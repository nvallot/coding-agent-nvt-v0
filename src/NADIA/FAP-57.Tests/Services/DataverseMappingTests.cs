using Xunit;
using FluentAssertions;
using Shared.Models;

namespace FAP_57.Tests.Services;

public class DataverseMappingTests
{
    [Fact]
    public void DataverseStagingPurchaseOrder_ShouldHaveCorrectStatusCode()
    {
        // Arrange & Act
        var entity = new DataverseStagingPurchaseOrder
        {
            PoNumber = "PO-12345"
        };

        // Assert
        entity.StatusCode.Should().Be(918860002); // Ready to be Processed
    }

    [Fact]
    public void DataverseStagingPurchaseOrder_ShouldFormatDatesCorrectly()
    {
        // Arrange
        var firstDelivery = new DateTime(2026, 06, 15);
        var lastDelivery = new DateTime(2026, 12, 31);

        // Act
        var entity = new DataverseStagingPurchaseOrder
        {
            FirstDelivery = firstDelivery.ToString("yyyy-MM-dd"),
            LastDelivery = lastDelivery.ToString("yyyy-MM-dd")
        };

        // Assert
        entity.FirstDelivery.Should().Be("2026-06-15");
        entity.LastDelivery.Should().Be("2026-12-31");
    }
}
