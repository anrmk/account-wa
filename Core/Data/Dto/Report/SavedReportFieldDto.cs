using System;

namespace Core.Data.Dto {
    public class SavedReportFieldDto {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public decimal? Amount { get; set; }
        public long? ReportId { get; set; }

        [Obsolete]
        public string Value { get; set; }

        [Obsolete]
        public string Code { get; set; }
    }
}
