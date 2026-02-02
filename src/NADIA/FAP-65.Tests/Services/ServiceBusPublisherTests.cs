using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using FAP_65.RetrivePOVendor.Services;
using Shared.Models;
using System.Text.Json;

namespace FAP_65.Tests.Services;

public class ServiceBusPublisherTests
{
    [Fact]
    public void PurchaseOrderMessage_ShouldSerialize_WithCamelCase()
    {
        // Arrange
        var message = new PurchaseOrderMessage
        {
            PoNumber = "PO-12345",
            MdmNumber = "MDM-67890",
            PkmGuid = "12345678-1234-1234-1234-123456789012",
            Amount = 250000.50m,
            ProductCode = "PKG.123.456"
        };

        // Act
        var json = JsonSerializer.Serialize(message, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        // Assert
        json.Should().Contain("\"poNumber\":\"PO-12345\"");
        json.Should().Contain("\"mdmNumber\":\"MDM-67890\"");
        json.Should().Contain("\"pkmGuid\":\"12345678-1234-1234-1234-123456789012\"");
        json.Should().Contain("\"amount\":250000.5");
        json.Should().Contain("\"productCode\":\"PKG.123.456\"");
    }
}
