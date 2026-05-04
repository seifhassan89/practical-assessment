using System.Text.Json.Serialization;

namespace CacheImplementation.Infrastructure.Services;

public sealed record StarWarsPersonApiResponse
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("height")]
    public string Height { get; init; } = string.Empty;

    [JsonPropertyName("mass")]
    public string Mass { get; init; } = string.Empty;

    [JsonPropertyName("hair_color")]
    public string HairColor { get; init; } = string.Empty;

    [JsonPropertyName("skin_color")]
    public string SkinColor { get; init; } = string.Empty;

    [JsonPropertyName("eye_color")]
    public string EyeColor { get; init; } = string.Empty;

    [JsonPropertyName("birth_year")]
    public string BirthYear { get; init; } = string.Empty;

    [JsonPropertyName("gender")]
    public string Gender { get; init; } = string.Empty;
}