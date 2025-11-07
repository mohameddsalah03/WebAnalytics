using Analytics.APIs.Controllers.Controllers.Base;
using Analytics.Core.Application.Abstraction.Services.Reports;
using Analytics.Shared.DTOs.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Analytics.APIs.Controllers.Controllers.Reports;

[Authorize]
public class ReportsController : BaseApiController
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

   
    // Get overview report (totals across all pages and dates)
    [HttpGet("overview")]
    public async Task<ActionResult<OverviewDto>> GetOverview()
    {
        var overview = await _reportService.GetOverviewAsync();
        return Ok(overview);
    }

    // Get per-page report (grouped by page with totals/averages)
    [HttpGet("pages")]
    public async Task<ActionResult<List<PageReportDto>>> GetPageReports()
    {
        var pageReports = await _reportService.GetPageReportAsync();
        return Ok(pageReports);
    }

    

}