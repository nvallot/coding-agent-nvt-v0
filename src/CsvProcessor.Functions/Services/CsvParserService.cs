#nullable enable

using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using CsvProcessor.Functions.Configuration;
using CsvProcessor.Functions.Models;

namespace CsvProcessor.Functions.Services;

/// <summary>
/// Parses CSV files using CsvHelper and validates each line with FluentValidation.
/// Implements FR-002 (Parsing CSV) and FR-003 (Validation des Données).
/// </summary>
public sealed class CsvParserService : ICsvParserService
{
    private readonly IValidator<CsvOrderLine> _validator;
    private readonly AppSettings _settings;
    private readonly ILogger<CsvParserService> _logger;

    public CsvParserService(
        IValidator<CsvOrderLine> validator,
        IOptions<AppSettings> settings,
        ILogger<CsvParserService> logger)
    {
        _validator = validator;
        _settings = settings.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<CsvParseResult> ParseAsync(Stream stream, CancellationToken ct = default)
    {
        var validLines = new List<CsvOrderLine>();
        var invalidLines = new List<ValidationError>();
        var lineNumber = 0;

        // Try to detect separator from first line
        var separator = await DetectSeparatorAsync(stream, ct);
        stream.Position = 0; // Reset stream after detection

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = separator.ToString(),
            HasHeaderRecord = true,
            MissingFieldFound = null,
            BadDataFound = context =>
            {
                _logger.LogWarning("Bad data found at line {Line}: {Data}", 
                    context.Context?.Parser?.Row, context.RawRecord);
            }
        };

        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, config);

        // Read header
        await csv.ReadAsync();
        csv.ReadHeader();

        while (await csv.ReadAsync())
        {
            ct.ThrowIfCancellationRequested();
            lineNumber++;

            try
            {
                var record = csv.GetRecord<CsvOrderLine>();
                if (record is null)
                {
                    invalidLines.Add(new ValidationError
                    {
                        LineNumber = lineNumber,
                        RawContent = csv.Context.Parser?.RawRecord,
                        Errors = ["Failed to parse record"]
                    });
                    continue;
                }

                var validationResult = await _validator.ValidateAsync(record, ct);
                if (validationResult.IsValid)
                {
                    validLines.Add(record);
                }
                else
                {
                    invalidLines.Add(new ValidationError
                    {
                        LineNumber = lineNumber,
                        RawContent = csv.Context.Parser?.RawRecord,
                        Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
                    });

                    _logger.LogWarning(
                        "Validation failed at line {LineNumber}: {Errors}",
                        lineNumber,
                        string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
                }
            }
            catch (Exception ex)
            {
                invalidLines.Add(new ValidationError
                {
                    LineNumber = lineNumber,
                    RawContent = csv.Context.Parser?.RawRecord,
                    Errors = [$"Parse error: {ex.Message}"]
                });

                _logger.LogWarning(ex, "Parse error at line {LineNumber}", lineNumber);
            }
        }

        _logger.LogInformation(
            "CSV parsing complete: {ValidCount} valid, {InvalidCount} invalid",
            validLines.Count, invalidLines.Count);

        return new CsvParseResult
        {
            ValidLines = validLines,
            InvalidLines = invalidLines
        };
    }

    private async Task<char> DetectSeparatorAsync(Stream stream, CancellationToken ct)
    {
        using var reader = new StreamReader(stream, leaveOpen: true);
        var firstLine = await reader.ReadLineAsync(ct);

        if (string.IsNullOrEmpty(firstLine))
        {
            return _settings.CsvSeparator[0];
        }

        // Count occurrences of each separator
        var semicolonCount = firstLine.Count(c => c == ';');
        var commaCount = firstLine.Count(c => c == ',');

        if (semicolonCount > commaCount && semicolonCount >= 3)
        {
            _logger.LogDebug("Detected semicolon separator");
            return ';';
        }

        if (commaCount > semicolonCount && commaCount >= 3 && _settings.AllowCommaSeparator)
        {
            _logger.LogDebug("Detected comma separator");
            return ',';
        }

        // Default to configured separator
        return _settings.CsvSeparator[0];
    }
}
