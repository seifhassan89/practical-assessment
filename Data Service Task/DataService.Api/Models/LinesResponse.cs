namespace DataService.Api.Models;

/// <summary>
/// The response payload returned by <c>GET /api/lines</c>.
/// </summary>
/// <param name="Count">Total number of lines returned.</param>
/// <param name="Lines">The individual lines read from the data source.</param>
public sealed record LinesResponse(int Count, IReadOnlyList<string> Lines);
