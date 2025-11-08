using Analytics.Core.Application.Abstraction.Services.Reports;
using Analytics.Core.Domain.Contracts.Persistence;
using Analytics.Shared.DTOs.Reports;

namespace Analytics.Core.Application.Services.Reports
{
    internal class ReportService(
       IUnitOfWork _unitOfWork
        ) : IReportService
    {
        
        public async Task<OverviewDto> GetOverviewAsync()
        {

            var allStats = await _unitOfWork.DailyStatsRepo.GetAllStatsAsync();

            if (!allStats.Any())
            {
                //_logger.LogWarning("No statistics available");
                return new OverviewDto
                {
                    TotalUsers = 0,
                    TotalSessions = 0,
                    TotalViews = 0,
                    AvgPerformance = 0
                };
            }

            var overview = new OverviewDto
            {
                TotalUsers = allStats.Sum(x => x.TotalUsers),
                AvgPerformance = allStats.Average(x => x.AvgPerformance),
                TotalSessions = allStats.Sum(x => x.TotalSessions),
                TotalViews = allStats.Sum(x => x.TotalViews),
            };

            return overview;
        }

        public async Task<IEnumerable<PageReportDto>> GetPageReportAsync()
        {
            var allData = await _unitOfWork.RawDataRepo.GetGroupedByPageAsync();

            if (!allData.Any())
            {
                //_logger.LogWarning("No raw data available");
                return new List<PageReportDto>();
            }

            var pageReports = allData
            .GroupBy(r => r.Page)
            .Select(g => new PageReportDto
            {
                Page = g.Key,
                TotalUsers = g.Sum(r => r.Users),
                TotalSessions = g.Sum(r => r.Sessions),
                TotalViews = g.Sum(r => r.Views),
                AvgPerformance = g.Average(r => r.PerformanceScore)
            })
            .OrderByDescending(p => p.TotalViews)
            .ToList();

            return pageReports;
        }
    }
}
