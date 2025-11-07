using Analytics.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Analytics.Infrastructure.Persistence.Data.Config
{
    public class RawDataConfiguration : IEntityTypeConfiguration<RawAnalyticsData>
    {
        public void Configure(EntityTypeBuilder<RawAnalyticsData> builder)
        {
           

            builder.Property(r => r.Page)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(r => r.Date)
                .IsRequired();

            // Composite index for faster queries
            builder.HasIndex(r => new { r.Date, r.Page })
                .IsUnique();
        }
    }
}
