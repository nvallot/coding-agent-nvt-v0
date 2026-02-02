using System.Text.Json.Serialization;

namespace Shared.Models;

/// <summary>
/// Réponse de l'API Lucy pour les informations utilisateur PKM
/// Endpoint: GET /api/users/{pkmGuid}
/// </summary>
public class LucyUserResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("givenName")]
    public string GivenName { get; set; } = string.Empty;

    [JsonPropertyName("surName")]
    public string SurName { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;
}
