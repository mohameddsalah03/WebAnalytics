using Analytics.Core.Domain.Entities.Base;

namespace Analytics.Core.Domain.Entities
{
    public class RawAnalyticsData : BaseEntity<int>
    {
        public DateTime Date { get; set; }
        public string Page { get; set; } = string.Empty;
        public int Users { get; set; } 
        public int Sessions { get; set; } 
        public int Views { get; set; } 
        public double PerformanceScore { get; set; }   
        public int LcpMs { get; set; }  // Largest Contentful Paint (milliseconds)


    }
}
