using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Entities {
    [Table(name: "Payments")]
    public class PaymentEntity: AuditableEntity<long> {
        public int Amount { get; set; }
        public DateTime Date { get; set; }

        [ForeignKey("Invoice")]
        [Column("Invoice_Id")]
        public long? InvoiceId { get; set; }
        public virtual InvoiceEntity Invoice { get; set; }
    }
}
