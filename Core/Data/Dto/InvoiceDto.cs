using System;
using System.Collections.Generic;

namespace Core.Data.Dto {
    public class InvoiceDto {
        public long Id { get; set; }
        public string No { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TaxRate { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }

        public long? CompanyId { get; set; }
        public virtual CompanyDto Company { get; set; }

        public long? CustomerId { get; set; }
        public virtual CustomerDto Customer { get; set; }

        public virtual List<PaymentDto> Payments { get; set; }

        public bool IsDraft { get; set; }

        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
