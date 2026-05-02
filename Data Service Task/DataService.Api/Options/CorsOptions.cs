namespace DataService.Api.Options;

public sealed class CorsOptions
{
    public const string SectionName = "Cors";

    /// <summary>
    /// List of allowed origins. Use <c>["*"]</c> to permit any origin (development only).
    /// </summary>
    public string[] AllowedOrigins { get; init; } = [];

    /// <summary>
    /// Allowed HTTP methods. Defaults to all methods when empty.
    /// </summary>
    public string[] AllowedMethods { get; init; } = [];

    /// <summary>
    /// Allowed HTTP headers. Defaults to all headers when empty.
    /// </summary>
    public string[] AllowedHeaders { get; init; } = [];
}
