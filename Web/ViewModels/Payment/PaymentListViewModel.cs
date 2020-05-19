using System;

namespace Web.ViewModels {
    public class PaymentListViewModel {
        public long Id { get; set; }
        public string No { get; set; }
        public string InvoiceNo { get; set; }
        public long InvoiceId { get; set; }
        public decimal InvoiceAmount { get; set; }
        public string CompanyName { get; set; }
        public string Amount { get; set; }

        public string Date { get; set; }

        public string DueDate { get; set; }

        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
