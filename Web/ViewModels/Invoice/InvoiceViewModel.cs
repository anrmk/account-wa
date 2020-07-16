using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Web.ViewModels {
    public class InvoiceViewModel {
        public long Id { get; set; }

        [Required]
        [MaxLength(16)]
        [Display(Name = "Invoice No")]
        public string No { get; set; }

        [Required]
        [Display(Name = "Subtotal")]
        public decimal Subtotal { get; set; }

        [Range(0, 100)]
        [Display(Name = "Tax rate")]
        public decimal TaxRate { get; set; }

        [DisplayFormat(DataFormatString = "{0:#.##}", ApplyFormatInEditMode = true)]
        public decimal Amount => Subtotal * (1 + TaxRate / 100);

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name = "Due date")]
        public DateTime DueDate { get; set; }

        //public List<PaymentViewModel> Payments { get; set; }

        //[DisplayFormat(DataFormatString = "{0:#.##}", ApplyFormatInEditMode = true)]
        //public decimal PaymentAmount { get; set; }

        //[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        //public DateTime? PaymentDate { get; set; }

        [Required]
        [Display(Name = "Business name")]
        public long? CustomerId { get; set; }

        public CustomerViewModel Customer { get; set; }

        [Display(Name = "Company")]
        public long? CompanyId { get; set; }
        public CompanyViewModel Company { get; set; }
    }

    public class InvoiceImportViewModel {
        [Required]
        [Display(Name = "Amount")]
        [SpecialName()]
        public decimal Subtotal { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [SpecialName()]
        public DateTime Date { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Due date")]
        [SpecialName()]
        public DateTime DueDate { get; set; }

        [Required]
        [Display(Name = "Customer No")]
        [SpecialName()]
        public string CustomerNo { get; set; }

        [Required]
        [Display(Name = "Payment amount")]
        [SpecialName()]
        public decimal? PaymentAmount { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Payment date")]
        [SpecialName()]
        public DateTime? PaymentDate { get; set; }
    }
}
