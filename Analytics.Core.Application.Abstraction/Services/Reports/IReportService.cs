using Analytics.Shared.DTOs.Reports;
using System.Text.RegularExpressions;

namespace Analytics.Core.Application.Abstraction.Services.Reports
{
    public interface IReportService
    {
        Task<OverviewDto> GetOverviewAsync();

        Task<IEnumerable<PageReportDto>> GetPageReportAsync();
    }
}
