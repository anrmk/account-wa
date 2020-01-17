using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels {
    public class InvoiceViewModelList {
        public long Id { get; set; }
        public string No { get; set; }
        public double Subtotal { get; set; }
        public double TaxRate { get; set; }
        public string Amount => (Subtotal * (1 + TaxRate / 100)).ToString("0.##");
        public string Date { get; set; }
        public string DueDate { get; set; }

        public double PaymentAmount { get; set; }
        public string PaymentDate { get; set; }
        public string CompanyName { get; set; }
        public string CustomerName { get; set; }
    }

    public class InvoiceViewModel {
        public long Id { get; set; }

        [Required]
        [MaxLength(8)]
        [Display(Name = "Invoice No")]
        public string No { get; set; }

        [Required]
        [Display(Name = "Subtotal")]
        public decimal Subtotal { get; set; }

        [Range(0, 100)]
        [Display(Name = "Tax Rate")]
        public decimal TaxRate { get; set; }

        [DisplayFormat(DataFormatString = "{0:#.##}", ApplyFormatInEditMode = true)]
        public decimal Amount => Subtotal * (1 + TaxRate / 100);

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }

        public List<PaymentViewModel> Payment { get; set; }

        [DisplayFormat(DataFormatString = "{0:#.##}", ApplyFormatInEditMode = true)]
        public decimal PaymentAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? PaymentDate { get; set; }

        [Display(Name = "Business name")]
        public long? CustomerId { get; set; }
        public CustomerViewModel Customer { get; set; }

        [Display(Name = "Company")]
        public long? CompanyId { get; set; }
        public CompanyViewModel Company { get; set; }
    }
}
