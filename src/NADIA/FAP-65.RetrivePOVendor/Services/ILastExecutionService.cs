namespace FAP_65.RetrivePOVendor.Services;

/// <summary>
/// Service pour gérer la dernière date d'exécution dans Azure Table Storage
/// </summary>
public interface ILastExecutionService
{
    Task<(DateTime? lastExecDate, TimeSpan lastExecTime)> GetLastExecutionAsync();
    Task UpdateLastExecutionAsync(DateTime date, TimeSpan time);
}
