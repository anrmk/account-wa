using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Entities {
    [Table(name: "Customers")]
    public class CustomerEntity: EntityBase<long> {
        [Required]
        [MaxLength(6)]
        public string AccountNumber { get; set; }

        [Required]
        [MaxLength(64)]
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public string PhoneNumber { get; set; }

        public double CreditLimit { get; set; }

        public double CreditUtilized { get; set; }

        public string Terms { get; set; }

        [ForeignKey("Address")]
        [Column("CustomerAddress_Id")]
        public long? AddressId { get; set; }
        public virtual CustomerAddressEntity Address { get; set; }

        [ForeignKey("Company")]
        [Column("Company_Id")]
        public long? CompanyId{ get; set; }
        public virtual CompanyEntity Company { get; set; }

        public virtual ICollection<InvoiceEntity> Invoices { get; set; }
    }

    //public enum TermsEnum {
    //    NET_30 = 0,
    //    CASH_ACCOUNT = 1
    //}
}
