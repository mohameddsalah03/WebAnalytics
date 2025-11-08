using Analytics.Core.Application.Abstraction.Services.Auth;
using Analytics.Core.Application.Abstraction.Services.Reports;
using Analytics.Core.Application.Services.Auth;
using Analytics.Core.Application.Services.Reports;
using Microsoft.Extensions.DependencyInjection;

namespace Analytics.Core.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        //// AutoMapper
        //services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());


        // Authentication Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        // Report Services
        services.AddScoped<IReportService, ReportService>();

        return services;
    }
}