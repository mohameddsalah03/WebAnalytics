using Analytics.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Analytics.APIs.Extensions
{
    public static class InitializerExtensions
    {
        public static async Task<WebApplication> InitializeDbContext(this WebApplication app)
        {
            using var scope = app.Services.CreateAsyncScope();
            var services = scope.ServiceProvider;

            var dbContext = services.GetRequiredService<AppDbContext>();
            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
                await dbContext.Database.MigrateAsync(); //Update-Database

            return app;
        }
    }

}
