using System;

namespace Core.Data.Dto {
    [Obsolete]
    public class SavedReportPlanFieldDto {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Code { get; set; }

        public long? ReportId { get; set; }
    }
}
