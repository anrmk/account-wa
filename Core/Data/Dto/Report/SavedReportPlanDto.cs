using System;
using System.Collections.Generic;

namespace Core.Data.Dto {
    public class SavedReportPlanDto {
        public long Id { get; set; }
        public string Name { get; set; }
        public long? CompanyId { get; set; }
        public DateTime Date { get; set; }
        public int NumberOfPeriods { get; set; }
        public Guid ApplicationUserId { get; set; }

        public virtual ICollection<SavedReportPlanFieldDto> Fields { get; set; }
    }
}
