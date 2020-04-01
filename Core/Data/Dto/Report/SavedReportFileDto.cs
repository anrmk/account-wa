namespace Core.Data.Dto {
    public class SavedReportFileDto {
        public long Id { get; set; }
        public string Name { get; set; }
        public byte[] File { get; set; }
        public long? ReportId { get; set; }
    }
}
