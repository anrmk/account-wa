using System;
using System.Collections.Generic;

using Core.Extension;

namespace Core.Data.Dto {
    public class InvoiceFilterDto: PagerFilter {
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
    }
}
