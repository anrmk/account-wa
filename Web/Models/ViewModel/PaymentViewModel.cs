using System;

namespace Web.ViewModels {
    public class PaymentViewModel {
        public long Id { get; set; }
        public long InvoiceId { get; set; }
        public int Amount { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;
        public string DisplayDate => Date.ToString("MMMM/dd/yyyy");
    }
}
