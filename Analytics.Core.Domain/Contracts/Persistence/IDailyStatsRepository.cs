using Analytics.Core.Domain.Entities;

namespace Analytics.Core.Domain.Contracts.Persistence;

public interface IDailyStatsRepository : IGenericRepository<DailyStatistics, int>
{
    Task<DailyStatistics?> GetByDateAsync(DateTime date);
    Task<IEnumerable<DailyStatistics>> GetAllStatsAsync();
    Task<DailyStatistics> UpsertAsync(DailyStatistics stats);
}