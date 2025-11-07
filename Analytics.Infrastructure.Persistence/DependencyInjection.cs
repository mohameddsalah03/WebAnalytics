using Analytics.Core.Domain.Contracts.Persistence;
using Analytics.Infrastructure.Persistence.Data;
using Analytics.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Analytics.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistenceServices(
        this IServiceCollection services,
        IConfiguration configuration
        )
    {
        // Database Context
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("WebAnalyticsContext"));
        });


        // Specific Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRawDataRepository, RawDataRepository>();
        services.AddScoped<IDailyStatsRepository, DailyStatsRepository>();
        
        // Unit of Work Pattern
        services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();

        return services;
    }
}