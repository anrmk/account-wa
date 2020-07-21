using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Entities {
    [Table(name: "SavedReportPlans")]
    public class SavedReportPlanEntity: EntityBase<long> {
        [Column("Company_Id")]
        public long? CompanyId { get; set; }

        public int NumberOfPeriods { get; set; }

        public DateTime Date { get; set; }

        public virtual ICollection<SavedReportPlanFieldEntity> Fields { get; set; }
    }
}
