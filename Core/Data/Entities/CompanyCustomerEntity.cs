using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Entities {
    [Table(name: "CompanyCustomers")]
    public class CompanyCustomerEntity {
        [Column("Company_Id", Order = 0)]
        public long CompanyId { get; set; }

        public virtual CompanyEntity Company { get; set; }

        [Column("Customer_Id", Order = 1)]
        public long CustomerId { get; set; }

        public virtual CustomerEntity Customer { get; set; }
    }
}
