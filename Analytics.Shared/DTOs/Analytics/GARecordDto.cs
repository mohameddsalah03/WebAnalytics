namespace Analytics.Shared.DTOs.Analytics
{
    public class GARecordDto
    {
        public DateTime Date { get; set; }
        public string Page { get; set; } = string.Empty;
        public int Users { get; set; }
        public int Sessions { get; set; }
        public int Views { get; set; }
    }
}
