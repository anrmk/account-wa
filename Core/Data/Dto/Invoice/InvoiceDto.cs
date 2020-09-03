using System;
using System.Collections.Generic;

using Core.Extension;

namespace Core.Data.Dto {
    public class InvoiceDto {
        public long Id { get; set; }
        public string No { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TaxRate { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }

        public long? CompanyId { get; set; }
        public CompanyDto Company { get; set; }

        public long CustomerId { get; set; }
        public CustomerDto Customer { get; set; }

        public virtual ICollection<PaymentDto> Payments { get; set; }

        public bool IsDraft { get; set; }

        public string Status {
            get {
                var totalPaymentAmount = Payments.TotalAmount();
                if(Subtotal <= totalPaymentAmount) {
                    return "Paid";
                } else if(totalPaymentAmount > 0 && totalPaymentAmount < Subtotal) {
                    return "Partially paid";
                } else {
                    return "Unpaid";
                }
            }
        }

        public decimal Balance => Payments.TotalAmount() - Subtotal;

        public decimal Amount => Subtotal * (1 + TaxRate / 100);

        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
