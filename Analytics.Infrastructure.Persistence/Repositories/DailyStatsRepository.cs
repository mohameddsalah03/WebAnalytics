using Analytics.Core.Domain.Contracts.Persistence;
using Analytics.Core.Domain.Entities;
using Analytics.Infrastructure.Persistence.Data;
using Analytics.Infrastructure.Persistence.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace Analytics.Infrastructure.Persistence.Repositories;

internal class DailyStatsRepository : GenericRepository<DailyStatistics, int>, IDailyStatsRepository
{
    private readonly AppDbContext _context;

    public DailyStatsRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<DailyStatistics?> GetByDateAsync(DateTime date)
        =>  await _context.DailyStatistics
            .FirstOrDefaultAsync(s => s.Date.Date == date.Date);
    

    public async Task<IEnumerable<DailyStatistics>> GetAllStatsAsync()
        => await _context.DailyStatistics
            .AsNoTracking()
            .OrderBy(s => s.Date)
            .ToListAsync();

    public async Task<DailyStatistics> UpsertAsync(DailyStatistics stats)
    {
        
        var existing = await _context.DailyStatistics
            .FirstOrDefaultAsync(s => s.Date.Date == stats.Date.Date); // by default AsTracking(). 

        if (existing != null)
        {
            //  Update existing
            existing.TotalUsers = stats.TotalUsers;
            existing.TotalSessions = stats.TotalSessions;
            existing.TotalViews = stats.TotalViews;
            existing.AvgPerformance = stats.AvgPerformance;
            existing.UpdatedAt = DateTime.UtcNow;

            // don't need to make Update() => entity already tracked
            // _context.DailyStatistics.Update(existing);

            return existing;
        }
        else
        {
            //  Add new
            await _context.DailyStatistics.AddAsync(stats);
            return stats;
        }
    }
}