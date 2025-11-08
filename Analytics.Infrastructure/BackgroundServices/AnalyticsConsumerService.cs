using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Analytics.Core.Domain.Contracts.Infrastructure;
using Analytics.Shared.Settings;
using Analytics.Core.Domain.Contracts.Persistence;
using Analytics.Shared.DTOs.Analytics;
using Analytics.Core.Domain.Entities;


namespace Analytics.Infrastructure.BackgroundServices;

public class AnalyticsConsumerService : BackgroundService
{
    private readonly IMessageBroker _messageBroker;
    private readonly IServiceProvider _serviceProvider;
    private readonly RabbitMQSettings _settings;

    public AnalyticsConsumerService(
        IMessageBroker messageBroker,
        IServiceProvider serviceProvider,
        IOptions<RabbitMQSettings> settings
        )
    {
        _messageBroker = messageBroker;
        _serviceProvider = serviceProvider;
        _settings = settings.Value;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() =>
        {
            _messageBroker.StartConsuming(_settings.Queue, async (message) =>
            {
                return await ProcessMessageWithRetryAsync(message, maxRetries: 3, stoppingToken);
            });

            stoppingToken.WaitHandle.WaitOne();
        }, stoppingToken);
    }

    private async Task<bool> ProcessMessageWithRetryAsync(
        string message,
        int maxRetries,
        CancellationToken cancellationToken
        )
    {
        int attempt = 0;

        while (attempt < maxRetries && !cancellationToken.IsCancellationRequested)
        {
            try
            {
                attempt++;
                await ProcessMessageAsync(message);
                return true;
            }
            catch (Exception )
            {

                if (attempt < maxRetries)
                {
                    var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt));
                    await Task.Delay(delay, cancellationToken);
                }
            }
        }

        return false;
    }

    private async Task ProcessMessageAsync(string message)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var data = JsonSerializer.Deserialize<AnalyticsMessageDto>(message);
        if (data is null)  
        {
            throw new InvalidOperationException("Failed to deserialize message");
        }

        try
        {
            var rawData = new RawAnalyticsData
            {
                Date = data.Date.Date,
                Page = data.Page,
                Users = data.Users,
                Sessions = data.Sessions,
                Views = data.Views,
                PerformanceScore = data.PerformanceScore,
                LcpMs = data.LcpMs
            };

            //  Check if this record already exists
            var exists = await unitOfWork.RawDataRepo.ExistsAsync(data.Date.Date, data.Page);
            if (!exists)
            {
                await unitOfWork.RawDataRepo.AddAsync(rawData);
                await unitOfWork.CompleteAsync();
                Console.WriteLine($"[Consumer] Record saved: {data.Page} - {data.Date:yyyy-MM-dd}");
            }
            else
            {
                Console.WriteLine($"[Consumer] Duplicate record skipped: {data.Page} - {data.Date:yyyy-MM-dd}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Consumer] Error processing message: {ex.Message}");
            throw;
        }

        // 2️ Always aggregate
        await AggregateDailyStatsAsync(unitOfWork, data.Date.Date);
    }

    private async Task AggregateDailyStatsAsync(
        IUnitOfWork unitOfWork,
        DateTime date
        )
    {

        // Get all raw data for this date
        var dailyData = await unitOfWork.RawDataRepo.GetByDateAsync(date);

        if (!dailyData.Any())
        {
            return;
        }

        // Calculate aggregated stats
        var stats = new DailyStatistics
        {
            Date = date.Date,
            TotalUsers = dailyData.Sum(d => d.Users),
            TotalSessions = dailyData.Sum(d => d.Sessions),
            TotalViews = dailyData.Sum(d => d.Views),
            AvgPerformance = dailyData.Average(d => d.PerformanceScore)
        };

        // Check if stats already exist
        var existingStats = await unitOfWork.DailyStatsRepo.GetByDateAsync(date);

        if (existingStats != null)
        {
            existingStats.TotalUsers = stats.TotalUsers;
            existingStats.TotalSessions = stats.TotalSessions;
            existingStats.TotalViews = stats.TotalViews;
            existingStats.AvgPerformance = stats.AvgPerformance;
            existingStats.UpdatedAt = DateTime.UtcNow;

            unitOfWork.DailyStatsRepo.Update(existingStats);
        }
        else
        {
            await unitOfWork.DailyStatsRepo.AddAsync(stats);
        }

        await unitOfWork.CompleteAsync();
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        return base.StopAsync(cancellationToken);
    }

}