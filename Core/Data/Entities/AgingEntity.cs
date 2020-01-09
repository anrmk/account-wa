using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Entities {
    public class AgingEntity: AuditableEntity<long> {
        public DateTime Period { get; set; }

        [ForeignKey("Company")]
        [Column("Company_Id")]
        public long? CompanyId { get; set; }
        public CompanyEntity Company { get; set; }
    }
}
