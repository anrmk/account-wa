using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Core.Data.Entities.Nsi;

namespace Core.Data.Entities {
    [Table(name: "Customers")]
    public class CustomerEntity: AuditableEntity<long> {
        [Required]
        [MaxLength(16)]
        [Column(name: "AccountNumber")]
        public string No { get; set; }

        [Required]
        [MaxLength(256)]
        public string Name { get; set; }

        [MaxLength(2048)]
        public string Description { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        public string Terms { get; set; }

        [ForeignKey("Address")]
        [Column("CustomerAddress_Id")]
        public long? AddressId { get; set; }
        public virtual CustomerAddressEntity Address { get; set; }

        [ForeignKey("Company")]
        [Column("Company_Id", Order = 0)]
        public long? CompanyId { get; set; }
        public virtual CompanyEntity Company { get; set; }

        [ForeignKey("Type")]
        [Column("CustomerType_Id")]
        public long? TypeId { get; set; }
        public virtual CustomerTypeEntity Type { get; set; }

        public virtual ICollection<InvoiceEntity> Invoices { get; set; }

        public virtual ICollection<CustomerCreditLimitEntity> CreditLimits { get; set; }

        public virtual ICollection<CustomerCreditUtilizedEntity> CreditUtilizeds { get; set; }

        public virtual ICollection<CustomerActivityEntity> Activities { get; set; }

        public virtual ICollection<CustomerTagLinkEntity> TagLinks { get; set; }

        [NotMapped]
        public int TotalInvoices { get; set; }

        [NotMapped]
        public int UnpaidInvoices { get; set; }

        [NotMapped]
        public int Recheck { get; set; }
    }

    //public class CustomerBulkEntity: CustomerEntity {
    //    public int Total { get; set; }
    //    public int Recheck { get; set; }
    //}
}
