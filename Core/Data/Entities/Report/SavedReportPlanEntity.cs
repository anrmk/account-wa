using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Entities {
    [Table(name: "SavedReportPlans")]
    public class SavedReportPlanEntity: EntityBase<long> {

        [Column("ApplicationUser_Id")]
        public Guid ApplicationUserId { get; set; }

        [ForeignKey("Company")]
        [Column("Company_Id")]
        public long? CompanyId { get; set; }
        public virtual CompanyEntity Company { get; set; }

        public int NumberOfPeriods { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public virtual ICollection<SavedReportPlanFieldEntity> Fields { get; set; }
    }
}
