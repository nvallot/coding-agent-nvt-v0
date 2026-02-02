using Shared.Models;

namespace FAP_57.SendPOSupplier.Services;

/// <summary>
/// Service pour récupérer les informations utilisateur depuis Lucy API
/// </summary>
public interface ILucyApiService
{
    Task<LucyUserResponse?> GetUserByIdAsync(string userId, string correlationId);
}
