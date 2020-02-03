using System;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels {
    public class PaymentViewModelList {
        public long Id { get; set; }
        public string Ref { get; set; }
        public string InvoiceNo { get; set; }
        public long InvoiceId { get; set; }
        public decimal InvoiceAmount { get; set; }
        public decimal Amount { get; set; }
    }

    public class PaymentViewModel {
        public long Id { get; set; }

        [Required]
        [Display(Name = "Reference no.")]
        [MaxLength(16)]
        public string Ref { get; set; }

        [Range(0, 100)]
        [Display(Name = "Payment Amount")]
        [DisplayFormat(DataFormatString = "{0:#.##}", ApplyFormatInEditMode = true)]
        public decimal Amount { get; set; }

        [Display(Name = "Payment Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now;

        //[Range(0, 100)]
        //[Display(Name = "Tax Rate")]
        //[Column(TypeName = "decimal(18, 3)")]
        //public double TaxRate { get; set; }

        [Display(Name = "Invoice Amount")]
        [DisplayFormat(DataFormatString = "{0:#.##}", ApplyFormatInEditMode = true)]
        public decimal InvoiceAmount { get; set; }

        [Display(Name = "Invoice")]
        public long? InvoiceId { get; set; }

        [Display(Name = "Invoice No")]
        public string InvoiceNo { get; set; }
    }
}
