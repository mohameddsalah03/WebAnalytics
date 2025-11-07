using Analytics.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Analytics.Infrastructure.Persistence.Data.Config
{
    public class DailyStatsConfiguration : IEntityTypeConfiguration<DailyStatistics>
    {
        public void Configure(EntityTypeBuilder<DailyStatistics> builder)
        {

            builder.Property(d => d.Date)
                .IsRequired();

            // Unique constraint on Date (one record per day)
            builder.HasIndex(d => d.Date)
                .IsUnique();
        }
    }
}
