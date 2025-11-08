namespace Analytics.Shared.DTOs.Reports
{
    public class PageReportDto
    {
        public string Page { get; set; } = string.Empty;
        public int TotalUsers { get; set; }
        public int TotalSessions { get; set; }
        public int TotalViews { get; set; }
        public double AvgPerformance { get; set; }
    }
}
