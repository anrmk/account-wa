using System.ComponentModel.DataAnnotations;
using Core.Extension;

namespace Web.ViewModels {
    public class InvoiceListViewModel {
        public long Id { get; set; }
        public string No { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TaxRate { get; set; }
        public string Amount => (Subtotal * (1 + TaxRate / 100)).ToCurrency();

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public string Date { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public string DueDate { get; set; }

        public string Balance { get; set; }

        public string PaymentAmount { get; set; }
        public string PaymentDate { get; set; }

        public long CompanyId { get; set; }
        public string CompanyName { get; set; }

        public long CustomerId { get; set; }
        public string CustomerName { get; set; }

        public string Status { get; set; }
    }
}
