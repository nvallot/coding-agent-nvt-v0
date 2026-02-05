#nullable enable

using FluentValidation;
using CsvProcessor.Functions.Models;

namespace CsvProcessor.Functions.Validators;

/// <summary>
/// Validates CSV order lines according to business rules.
/// Implements FR-003 (Validation des Données).
/// </summary>
public sealed class CsvOrderLineValidator : AbstractValidator<CsvOrderLine>
{
    public CsvOrderLineValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("OrderId is required")
            .Matches(@"^ORD-\d+$")
            .WithMessage("OrderId must match pattern ORD-XXX");

        RuleFor(x => x.CustomerEmail)
            .NotEmpty()
            .WithMessage("CustomerEmail is required")
            .EmailAddress()
            .WithMessage("CustomerEmail must be a valid email address");

        RuleFor(x => x.ProductCode)
            .NotEmpty()
            .WithMessage("ProductCode is required")
            .MaximumLength(50)
            .WithMessage("ProductCode must not exceed 50 characters");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0");

        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0)
            .WithMessage("UnitPrice must be greater than or equal to 0");

        RuleFor(x => x.OrderDate)
            .NotEmpty()
            .WithMessage("OrderDate is required")
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
            .WithMessage("OrderDate cannot be in the future");
    }
}
