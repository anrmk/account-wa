using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Entities {
    [Table(name: "SavedReports")]
    public class SavedReportFactEntity: AuditableEntity<long> {
       // public string Name { get; set; }

        [Column("ApplicationUser_Id")]
        public Guid ApplicationUserId { get; set; }

        [ForeignKey("Company")]
        [Column("Company_Id", Order = 0)]
        public long? CompanyId { get; set; }
        public virtual CompanyEntity Company { get; set; }

        public int NumberOfPeriods { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public bool IsPublished { get; set; }

        public virtual ICollection<SavedReportFactFieldEntity> Fields { get; set; }

        public virtual ICollection<SavedReportFileEntity> Files { get; set; }
    }
}
