using System;
using System.Collections.Generic;

namespace Core.Data.Dto {
    public class InvoiceConstructorDto {
        public long Id { get; set; }

        public long CompanyId { get; set; }

        public DateTime Date { get; set; }

        public long SearchCriteriaId { get; set; }

        public long SummaryRangeId { get; set; }

        public int Count { get; set; }

        public virtual ICollection<InvoiceDraftDto> Invoices { get; set; }
    }
}
