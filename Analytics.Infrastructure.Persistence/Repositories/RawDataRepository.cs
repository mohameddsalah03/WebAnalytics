using Analytics.Core.Domain.Contracts.Persistence;
using Analytics.Core.Domain.Entities;
using Analytics.Infrastructure.Persistence.Data;
using Analytics.Infrastructure.Persistence.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace Analytics.Infrastructure.Persistence.Repositories;

internal class RawDataRepository : GenericRepository<RawAnalyticsData, int>, IRawDataRepository
{
    private readonly AppDbContext _context;

    public RawDataRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RawAnalyticsData>> GetByDateAsync(DateTime date)
        => await _context.RawAnalyticsData
            .AsNoTracking()
            .Where(r => r.Date.Date == date.Date)
            .OrderBy(r => r.Page)
            .ToListAsync();
    

    public async Task<IEnumerable<RawAnalyticsData>> GetByPageAsync(string page)
        => await _context.RawAnalyticsData
            .AsNoTracking()
            .Where(r => r.Page == page)
            .OrderBy(r => r.Date)
            .ToListAsync();
    

    public async Task<IEnumerable<RawAnalyticsData>> GetByDateAndPageAsync(DateTime date, string page)
        => await _context.RawAnalyticsData
            .AsNoTracking()
            .Where(r => r.Date.Date == date.Date && r.Page == page)
            .ToListAsync();
    

    public async Task<IEnumerable<RawAnalyticsData>> GetGroupedByPageAsync()
        => await _context.RawAnalyticsData
            .AsNoTracking()
            .OrderBy(r => r.Page)
            .ThenBy(r => r.Date)
            .ToListAsync();

    public async Task<bool> ExistsAsync(DateTime date, string page)
        => await _context.RawAnalyticsData
        .AnyAsync(r => r.Date.Date == date.Date && r.Page == page);

}