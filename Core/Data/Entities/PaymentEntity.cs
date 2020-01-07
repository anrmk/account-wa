using System;

namespace Core.Data.Entities {
    public class PaymentEntity: EntityBase<long> {
        public int Amount { get; set; }
        public DateTime Date { get; set; }

        public InvoiceEntity Invoice { get; set; }
    }
}
