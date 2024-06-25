using AggregationApiAssignment.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AggregationApiAssignment.Controllers;

[Route("programming-news")]
public class ProgrammingNewsController : ControllerBase
{
    private readonly IDataAggregator _aggregator;

    public ProgrammingNewsController(IDataAggregator aggregator)
    {
        _aggregator = aggregator;
    }

    [HttpGet]
    public async Task<ActionResult<AggregatedData>> GetProgrammingNews([FromQuery] string query, [FromQuery] DateOnly? dateFrom, [FromQuery] DateOnly? dateTo)
    {
        try
        {
            var data = await _aggregator.AggregateTopHeadlinesAsync(query, dateFrom, dateTo);
            return Ok(data);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}