using System;
using System.Collections.Generic;

namespace Core.Data.Dto {
    public class InvoiceFilterDto: PagerFilterDto {
        public long? CompanyId { get; set; }
        public long? CustomerId { get; set; }
        public long? TypeId { get; set; }

        public DateTime? Date { get; set; }
        public int NumberOfPeriods { get; set; }

        //public int? From { get; set; }
        //public int? To { get; set; }
        public List<string> Periods { get; set; }
        public bool MoreThanOne { get; set; }

        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public DateTime? CreatedDateFrom { get; set; }
        public DateTime? CreatedDateTo { get; set; }
    }
}
