namespace Analytics.Core.Application.Abstraction.Common.Contracts.Infrastructure
{
    public interface IDataIngestionService
    {
        Task IngestDataAsync();
    }
}
