namespace Analytics.Shared.DTOs.Analytics
{
    public class PSIRecordDto
    {
        public DateTime Date { get; set; }
        public string Page { get; set; } = string.Empty;
        public double PerformanceScore { get; set; }
        public int LCP_ms { get; set; }
    }
}
