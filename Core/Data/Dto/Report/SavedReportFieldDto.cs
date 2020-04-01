namespace Core.Data.Dto {
    public class SavedReportFieldDto {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public long? ReportId { get; set; }
    }
}
