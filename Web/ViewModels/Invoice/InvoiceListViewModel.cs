using System;

using Core.Extension;

namespace Web.ViewModels {
    public class InvoiceListViewModel {
        public long Id { get; set; }
        public string No { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TaxRate { get; set; }
        public string Amount => (Subtotal * (1 + TaxRate / 100)).ToCurrency();

        public string Date { get; set; }

        public string DueDate { get; set; }

        public string Balance { get; set; }

        public string PaymentAmount { get; set; }
        public string PaymentDate { get; set; }

        public long CompanyId { get; set; }
        public string CompanyName { get; set; }

        public long CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerType { get; set; }
        public string[] CustomerTags { get; set; }
        public string CustomerCreatedDate { get; set; }

        public string Status { get; set; }

        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
