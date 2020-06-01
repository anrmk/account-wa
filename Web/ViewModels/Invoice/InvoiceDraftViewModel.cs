using System;

namespace Web.ViewModels {
    public class InvoiceDraftViewModel {
        public long Id { get; set; }

        public string No { get; set; }

        public decimal Subtotal { get; set; }

        public decimal TaxRate { get; set; }

        public DateTime Date { get; set; }

        public DateTime DueDate { get; set; }

        public long? ConstructorId { get; set; }
        public long? CompanyId { get; set; }
        public long? CustomerId { get; set; }
    }
}
