using System;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels {
    public class InvoiceViewModel {
        public long Id { get; set; }
        public double Amount { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }

        public string CustomerName { get; set; }
        public string CustomerAccountNumber { get; set; }
        public long? CustomerId { get; set; }

        public string PaymentAmount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public long? PaymentId { get; set; }

        public string DisplayPaymentDate => PaymentDate?.ToString("MMMM/dd/yyyy");
        public string DisplayDate => Date.ToString("MMMM yyyy");
        public string DisplayDueDate => DueDate.ToString("MMMM/dd/yyyy");
    }

    public class InvoiceGeneratorViewModel {
        [Required]
        public int Count { get; set; } = 1;

        [Required]
        public long Period { get; set; }

        // public virtual PeriodViewModel Perio { get; set; }
    }
}
