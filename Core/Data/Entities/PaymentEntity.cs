using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Entities {
    [Table(name: "Payments")]
    public class PaymentEntity: AuditableEntity<long> {
        [StringLength(16)]
        [Required]
        public string Ref { get; set; }

        [Range(1, 100)]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [ForeignKey("Invoice")]
        [Column("Invoice_Id")]
        public long? InvoiceId { get; set; }
        public virtual InvoiceEntity Invoice { get; set; }

        public bool IsDraft { get; set; } = true;
    }
}
