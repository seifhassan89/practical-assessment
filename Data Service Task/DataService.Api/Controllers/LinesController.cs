using DataService.Api.Abstractions;
using DataService.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace DataService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class LinesController : ControllerBase
{
    private readonly IDataService _dataService;

    public LinesController(IDataService dataService)
    {
        _dataService = dataService;
    }

    /// <summary>
    /// Returns all lines from the configured data source.
    /// </summary>
    /// <param name="cancellationToken">Propagated automatically by ASP.NET Core when the client disconnects.</param>
    /// <returns>All lines wrapped in a <see cref="SuccessApiResponse{T}"/> envelope.</returns>
    [HttpGet]
    [ProducesResponseType<SuccessApiResponse<LinesResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorApiResponse>(StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType<ErrorApiResponse>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<LinesResponse>> Get(CancellationToken cancellationToken)
    {
        IReadOnlyList<string> lines = await _dataService.GetLinesAsync(cancellationToken);
        return Ok(new LinesResponse(lines.Count, lines));
    }
}
