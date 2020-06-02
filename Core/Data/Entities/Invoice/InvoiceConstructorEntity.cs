using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Entities {
    [Table(name: "InvoiceConstructors")]
    public class InvoiceConstructorEntity: AuditableEntity<long> {

        public DateTime Date { get; set; }

        public long CompanyId { get; set; }

        public long SearchCriteriaId { get; set; }

        public long SummaryRangeId { get; set; }

        public int Count { get; set; }

        public virtual ICollection<InvoiceDraftEntity> Invoices { get; set; }
    }
}
