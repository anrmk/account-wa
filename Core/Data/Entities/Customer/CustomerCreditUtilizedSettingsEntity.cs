using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Core.Data.Enum;

namespace Core.Data.Entities {
    [Table(name: "CustomerCreditUtilizedSettings")]
    public class CustomerCreditUtilizedSettingsEntity: EntityBase<long> {
        [Required]
        public RoundType RoundType { get; set; }

        [ForeignKey("Company")]
        [Column("Company_Id", Order = 0)]
        public long? CompanyId { get; set; }
        public virtual CompanyEntity Company { get; set; }

        [Required]
        [ScaffoldColumn(false)]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }
    }
}
