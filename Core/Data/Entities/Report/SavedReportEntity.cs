using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Entities {
    [Table(name: "SavedReports")]
    public class SavedReportEntity: AuditableEntity<long> {
        public string Name { get; set; }

        [Column("ApplicationUser_Id")]
        public Guid ApplicationUserId { get; set; }

        [ForeignKey("Company")]
        [Column("Company_Id", Order = 0)]
        public long? CompanyId { get; set; }
        public virtual CompanyEntity Company { get; set; }

        public DateTime Date { get; set; }

        public int NumberOfPeriods { get; set; }

        public virtual ICollection<SavedReportFieldEntity> Fields { get; set; }

        public virtual ICollection<SavedReportFileEntity> Files { get; set; }
    }
}
