using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Entities {
    [Table(name: "CustomerRechecks")]
    public class CustomerRecheckEntity: AuditableEntity<long> {
        [ForeignKey("Customer")]
        [Column("Customer_Id")]
        public long? CustomerId { get; set; }
        public virtual CustomerEntity Customer { get; set; }

        [Required]
        public DateTime ReceivedDate { get; set; }

        [Required]
        public DateTime ReportDate { get; set; }
    }
}
