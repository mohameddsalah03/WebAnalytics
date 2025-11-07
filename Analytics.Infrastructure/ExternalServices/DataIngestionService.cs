using Analytics.Core.Application.Abstraction.Common.Contracts.Infrastructure;
using Analytics.Core.Domain.Contracts.Infrastructure;
using Analytics.Shared.DTOs.Analytics;
using Microsoft.AspNetCore.Hosting;
using System.Text.Json;

namespace Analytics.Infrastructure.ExternalServices;

public class DataIngestionService : IDataIngestionService
{
    private readonly IMessageBroker _messageBroker;
    private readonly string _dataPath;

    public DataIngestionService(
        IMessageBroker messageBroker,
        IWebHostEnvironment environment
        )
    {
        _messageBroker = messageBroker;
        _dataPath = Path.Combine(environment.ContentRootPath, "MockData");
    }

    public async Task IngestDataAsync()
    {
        try
        {
            var gaFilePath = Path.Combine(_dataPath, "ga_data.json");
            var psiFilePath = Path.Combine(_dataPath, "psi_data.json");

            if (!File.Exists(gaFilePath))
            {
                throw new FileNotFoundException("GA data file not found", gaFilePath);
            }

            if (!File.Exists(psiFilePath))
            {
                throw new FileNotFoundException("PSI data file not found", psiFilePath);
            }

            var gaData = await ReadJsonFileAsync<List<GARecordDto>>(gaFilePath);
            var psiData = await ReadJsonFileAsync<List<PSIRecordDto>>(psiFilePath);

            if (gaData is null || !gaData.Any())
            {
                return;
            }

            if (psiData is null || !psiData.Any())
            {
                return;
            }

            var combinedData = CombineData(gaData, psiData);

            foreach (var record in combinedData)
            {
                await _messageBroker.PublishAsync( record); 
            }

        }
        catch (Exception ex)
        {
            throw;
        }
    }

    #region Helper Methods


    private async Task<T?> ReadJsonFileAsync<T>(string filePath)
    {
        try
        {
            var json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    private List<AnalyticsMessageDto> CombineData(
        List<GARecordDto> gaData,
        List<PSIRecordDto> psiData)
    {
        var combined = new List<AnalyticsMessageDto>();

        foreach (var ga in gaData)
        {
            var psi = psiData.FirstOrDefault(p =>
                p.Page == ga.Page && p.Date.Date == ga.Date.Date);

            if (psi != null)
            {
                combined.Add(new AnalyticsMessageDto
                {
                    Page = ga.Page,
                    Date = ga.Date,
                    Users = ga.Users,
                    Sessions = ga.Sessions,
                    Views = ga.Views,
                    PerformanceScore = psi.PerformanceScore,
                    LcpMs = psi.LCP_ms
                });
            }
        }

        return combined;
    }


    #endregion
}