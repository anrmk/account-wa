namespace Core.Data.Dto {
    public class SavedReportPlanFieldDto {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Code { get; set; }

        public long? ReportId { get; set; }
    }
}
