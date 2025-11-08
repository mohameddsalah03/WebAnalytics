using Analytics.Core.Domain.Entities;

namespace Analytics.Core.Domain.Contracts.Persistence;

public interface IRawDataRepository : IGenericRepository<RawAnalyticsData, int>
{
    Task<IEnumerable<RawAnalyticsData>> GetByDateAsync(DateTime date);
    Task<IEnumerable<RawAnalyticsData>> GetByPageAsync(string page);
    Task<IEnumerable<RawAnalyticsData>> GetByDateAndPageAsync(DateTime date, string page);
    Task<IEnumerable<RawAnalyticsData>> GetGroupedByPageAsync();

    Task<bool> ExistsAsync(DateTime date, string page);


}