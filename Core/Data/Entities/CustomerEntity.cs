using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Entities {
    [Table(name: "Customers")]
    public class CustomerEntity: EntityBase<long> {
        [Required]
        [MaxLength(8)]
        public string AccountNumber { get; set; }

        [Required]
        [MaxLength(256)]
        public string Name { get; set; }

        [MaxLength(2048)]
        public string Description { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        public string Terms { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public double CreditLimit { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public double CreditUtilized { get; set; }

        [ForeignKey("Address")]
        [Column("CustomerAddress_Id")]
        public long? AddressId { get; set; }
        public virtual CustomerAddressEntity Address { get; set; }

        public virtual ICollection<CompanyCustomerEntity> CompanyCustomers { get; set; }

        public virtual ICollection<InvoiceEntity> Invoices { get; set; }
    }
}
