using System;

using Core.Extension;

namespace Core.Data.Dto {
    public class PaymentFilterDto: PagerFilter {
        public long? CompanyId { get; set; }

        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public DateTime? CreatedDateFrom { get; set; }
        public DateTime? CreatedDateTo { get; set; }
    }
}
