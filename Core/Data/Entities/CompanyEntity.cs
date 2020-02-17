using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AutoMapper.Configuration.Annotations;

namespace Core.Data.Entities {
    [Table(name: "Companies")]
    public class CompanyEntity: AuditableEntity<long> {
        [Required]
        [MaxLength(8)]
        public string No { get; set; }

        [Required]
        [MaxLength(256)]
        public string Name { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Range(0, 100)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TaxRate { get; set; }

        [ForeignKey("Address")]
        [Column("CompanyAddress_Id")]
        public long? AddressId { get; set; }
        public virtual CompanyAddressEntity Address { get; set; }

        [Ignore]
        public virtual ICollection<CustomerEntity> Customers { get; set; }

        [Ignore]
        public virtual ICollection<CompanySummaryRangeEntity> SummaryRange { get; set; }

    }
}
