using System.Collections.Generic;

namespace Core.Data.Dto {
    public class InvoiceConstructorSearchDto {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Sort { get; set; } = 0;

        public List<long> TagsIds { get; set; }
        public List<long> TypeIds { get; set; }
        public List<int> Recheck { get; set; }

        public int? CurrentInvoices { get; set; }
        public int? LateInvoices { get; set; }

        public bool RandomSort { get; set; }

        public bool OnlyNewCustomers { get; set; }
    }
}
