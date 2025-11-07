namespace Analytics.Core.Domain.Contracts.Persistence
{
    public interface IUnitOfWork : IAsyncDisposable
    {

        IUserRepository UsersRepo { get; }
        IRawDataRepository RawDataRepo { get; }
        IDailyStatsRepository DailyStatsRepo { get; }
        Task<int> CompleteAsync();
        
    }
}
