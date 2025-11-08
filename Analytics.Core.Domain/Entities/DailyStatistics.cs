using Analytics.Core.Domain.Entities.Base;

namespace Analytics.Core.Domain.Entities
{
    public class DailyStatistics : BaseEntity<int>
    {
        public DateTime Date { get; set; }
        public int TotalUsers { get; set; }
        public int TotalSessions { get; set; }
        public int TotalViews { get; set; }
        public double AvgPerformance { get; set; }
    }
}
