using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Entities {
    /// <summary>
    /// Счета
    /// </summary>
    public class InvoiceEntity: EntityBase<long> {
        public double Amount { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }

        [ForeignKey("Payment")]
        [Column("Payment_Id")]
        public long? PaymentId { get; set; }
        public virtual PaymentEntity Payment { get; set; }

        [ForeignKey("Customer")]
        [Column("Customer_Id")]
        public long? CustomerId { get; set; }
        public virtual CustomerEntity Customer { get; set; }
    }
}
