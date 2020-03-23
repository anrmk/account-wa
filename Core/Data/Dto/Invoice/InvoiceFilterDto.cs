using System;

using Core.Extension;

namespace Core.Data.Dto {
    public class InvoiceFilterDto: PagerFilter {
        public long? CompanyId { get; set; }
        public DateTime? Date { get; set; }
        public int NumberOfPeriods { get; set; }

        public int? From { get; set; }
        public int? To { get; set; }
    }
}
