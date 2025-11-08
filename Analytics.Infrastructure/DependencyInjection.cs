using Analytics.Core.Application.Abstraction.Common.Contracts.Infrastructure;
using Analytics.Core.Domain.Contracts.Infrastructure;
using Analytics.Infrastructure.BackgroundServices;
using Analytics.Infrastructure.ExternalServices;
using Analytics.Infrastructure.MessageBroker;
using Analytics.Shared.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Analytics.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // RabbitMQ Settings
        services.Configure<RabbitMQSettings>(
            configuration.GetSection("RabbitMQSettings"));

        // Message Broker (Singleton - shared connection)
        services.AddSingleton<IMessageBroker, RabbitMQBroker>();

        // Data Ingestion Service (moved from Application to Infrastructure)
        services.AddScoped<IDataIngestionService, DataIngestionService>();

        // Background Consumer Service
        services.AddHostedService<AnalyticsConsumerService>();

        return services;
    }
}