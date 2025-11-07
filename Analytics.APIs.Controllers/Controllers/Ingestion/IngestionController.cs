using Analytics.APIs.Controllers.Controllers.Base;
using Analytics.Core.Application.Abstraction.Common.Contracts.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Analytics.APIs.Controllers.Controllers.Ingestion;

public class IngestionController : BaseApiController
{
    private readonly IDataIngestionService _ingestionService;

    public IngestionController(IDataIngestionService ingestionService)
    {
        _ingestionService = ingestionService;
    }


    // Manually trigger data ingestion from JSON files
    [HttpPost("ingest")]
    public async Task<IActionResult> IngestData()
    {
        await _ingestionService.IngestDataAsync();
        return Accepted(new
        {
            message = "Data ingestion started successfully",
            timestamp = DateTime.UtcNow
        });
    }


    [HttpGet("health")]
    public IActionResult Health() => Ok("Healthy");



}