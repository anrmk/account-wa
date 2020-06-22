using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Entities {
    [Table(name: "CustomerCreditUtilized")]
    public class CustomerCreditUtilizedEntity: EntityBase<long> {
        [ForeignKey("Customer")]
        [Column("Customer_Id")]
        public long? CustomerId { get; set; }
        public CustomerEntity Customer { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Value { get; set; }

        public bool IsIgnored { get; set; }

        [ScaffoldColumn(false)]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
