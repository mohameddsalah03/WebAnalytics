using Analytics.Core.Domain.Contracts.Persistence;
using Analytics.Infrastructure.Persistence.Data;

namespace Analytics.Infrastructure.Persistence.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;

        public UnitOfWork(
        AppDbContext dbContext,
        IUserRepository users,
        IRawDataRepository rawData,
        IDailyStatsRepository dailyStats)
        {
            _dbContext = dbContext;
            UsersRepo = users;
            RawDataRepo = rawData;
            DailyStatsRepo = dailyStats;
        }


        public IUserRepository UsersRepo { get; }
        public IRawDataRepository RawDataRepo { get; }
        public IDailyStatsRepository DailyStatsRepo { get; }

        public async Task<int> CompleteAsync()
            => await _dbContext.SaveChangesAsync();    

        public async ValueTask DisposeAsync()
            => await _dbContext.DisposeAsync();

    }
}
