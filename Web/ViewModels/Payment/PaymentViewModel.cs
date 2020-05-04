using System;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels {
    public class PaymentViewModel {
        public long Id { get; set; }

        [Required]
        [Display(Name = "Reference no.")]
        [MaxLength(16)]
        public string No { get; set; }

        [Display(Name = "Payment amount")]
        [DisplayFormat(DataFormatString = "{0:#.##}", ApplyFormatInEditMode = true)]
        public decimal Amount { get; set; }

        [Display(Name = "Payment date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now;

        [Display(Name = "Invoice amount")]
        [DisplayFormat(DataFormatString = "{0:#.##}", ApplyFormatInEditMode = true)]
        public decimal InvoiceAmount { get; set; }

        [Display(Name = "Invoice")]
        public long? InvoiceId { get; set; }

        [Display(Name = "Invoice No")]
        public string InvoiceNo { get; set; }

        public long CustomerId { get; set; }
    }
}
