namespace Analytics.Shared.DTOs.Analytics
{
    public class AnalyticsMessageDto
    {
        public string Page { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int Users { get; set; }
        public int Sessions { get; set; }
        public int Views { get; set; }
        public double PerformanceScore { get; set; }
        public int LcpMs { get; set; }
    }
}
