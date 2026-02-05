#nullable enable

using CsvProcessor.Functions.Models;

namespace CsvProcessor.Functions.Services;

/// <summary>
/// Service for ensuring idempotent file processing.
/// Implements FR-006 (Idempotence) and ADR-002.
/// </summary>
public interface IIdempotencyService
{
    /// <summary>
    /// Checks if a file has already been processed based on its content hash.
    /// </summary>
    /// <param name="fileHash">MD5 hash of the file content.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>True if the file has already been processed.</returns>
    Task<bool> IsAlreadyProcessedAsync(string fileHash, CancellationToken ct = default);

    /// <summary>
    /// Marks a file as being processed (in-progress state).
    /// </summary>
    /// <param name="fileHash">MD5 hash of the file content.</param>
    /// <param name="fileName">Original file name.</param>
    /// <param name="ct">Cancellation token.</param>
    Task MarkAsProcessingAsync(string fileHash, string fileName, CancellationToken ct = default);

    /// <summary>
    /// Marks a file as successfully processed.
    /// </summary>
    /// <param name="fileHash">MD5 hash of the file content.</param>
    /// <param name="result">Processing result with statistics.</param>
    /// <param name="ct">Cancellation token.</param>
    Task MarkAsCompletedAsync(string fileHash, ProcessingResult result, CancellationToken ct = default);

    /// <summary>
    /// Marks a file as failed during processing.
    /// </summary>
    /// <param name="fileHash">MD5 hash of the file content.</param>
    /// <param name="errorMessage">Error message describing the failure.</param>
    /// <param name="ct">Cancellation token.</param>
    Task MarkAsFailedAsync(string fileHash, string errorMessage, CancellationToken ct = default);

    /// <summary>
    /// Computes MD5 hash of file content.
    /// </summary>
    /// <param name="stream">File stream.</param>
    /// <returns>Hexadecimal MD5 hash string.</returns>
    string ComputeHash(Stream stream);
}
