using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Entities {
    [Table(name: "CompanySummaryRange")]
    public class CompanySummaryRangeEntity: EntityBase<long> {
        [Column("Company_Id")]
        public long? CompanyId { get; set; }

        public virtual CompanyEntity Company { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal From { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal To { get; set; }
    }
}
