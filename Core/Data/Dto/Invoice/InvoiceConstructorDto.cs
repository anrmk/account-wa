using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Data.Dto {
    public class InvoiceConstructorDto {
        public long Id { get; set; }

        public long CompanyId { get; set; }

        public DateTime Date { get; set; }

        public long SearchCriteriaId { get; set; }

        public long SummaryRangeId { get; set; }

        public List<long> Customers { get; set; }

        public virtual ICollection<InvoiceDraftDto> Invoices { get; set; }

        public int Count => Invoices?.Count ?? 0;

        public decimal TotalAmount => Invoices?.Count > 0 ? Invoices.Sum(x => x.Subtotal) : 0;
    }
}
