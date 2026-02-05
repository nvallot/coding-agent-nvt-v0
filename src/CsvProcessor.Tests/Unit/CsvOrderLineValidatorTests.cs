#nullable enable

using FluentAssertions;
using CsvProcessor.Functions.Models;
using CsvProcessor.Functions.Validators;

namespace CsvProcessor.Tests.Unit;

public class CsvOrderLineValidatorTests
{
    private readonly CsvOrderLineValidator _validator = new();

    [Fact]
    public void Validate_ValidLine_ReturnsSuccess()
    {
        // Arrange
        var line = CreateValidLine();

        // Act
        var result = _validator.Validate(line);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("123")]
    [InlineData("ORDER-001")]
    public void Validate_InvalidOrderId_ReturnsError(string orderId)
    {
        // Arrange
        var line = CreateValidLine();
        line.OrderId = orderId;

        // Act
        var result = _validator.Validate(line);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "OrderId");
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid-email")]
    [InlineData("@domain.com")]
    [InlineData("user@")]
    public void Validate_InvalidEmail_ReturnsError(string email)
    {
        // Arrange
        var line = CreateValidLine();
        line.CustomerEmail = email;

        // Act
        var result = _validator.Validate(line);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CustomerEmail");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validate_InvalidQuantity_ReturnsError(int quantity)
    {
        // Arrange
        var line = CreateValidLine();
        line.Quantity = quantity;

        // Act
        var result = _validator.Validate(line);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Quantity");
    }

    [Fact]
    public void Validate_NegativeUnitPrice_ReturnsError()
    {
        // Arrange
        var line = CreateValidLine();
        line.UnitPrice = -10.00m;

        // Act
        var result = _validator.Validate(line);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "UnitPrice");
    }

    [Fact]
    public void Validate_FutureOrderDate_ReturnsError()
    {
        // Arrange
        var line = CreateValidLine();
        line.OrderDate = DateTime.UtcNow.AddDays(30);

        // Act
        var result = _validator.Validate(line);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "OrderDate");
    }

    [Fact]
    public void Validate_EmptyProductCode_ReturnsError()
    {
        // Arrange
        var line = CreateValidLine();
        line.ProductCode = "";

        // Act
        var result = _validator.Validate(line);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ProductCode");
    }

    [Fact]
    public void Validate_MultipleErrors_ReturnsAllErrors()
    {
        // Arrange
        var line = new CsvOrderLine
        {
            OrderId = "invalid",
            CustomerEmail = "invalid",
            ProductCode = "",
            Quantity = 0,
            UnitPrice = -1,
            OrderDate = DateTime.UtcNow.AddDays(30)
        };

        // Act
        var result = _validator.Validate(line);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThan(3);
    }

    private static CsvOrderLine CreateValidLine() => new()
    {
        OrderId = "ORD-001",
        CustomerEmail = "customer@example.com",
        ProductCode = "PROD-A",
        Quantity = 5,
        UnitPrice = 29.99m,
        OrderDate = DateTime.UtcNow.AddDays(-1)
    };
}
