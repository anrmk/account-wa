using System;
using System.Collections.Generic;

namespace Core.Data.Dto {
    public class CustomerFilterDto: PagerFilterDto {
        public long? CompanyId { get; set; }

        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public virtual ICollection<long?> TypeIds { get; set; }

        public virtual ICollection<long?> TagsIds { get; set; }

        public virtual ICollection<int?> Recheck { get; set; }

        public DateTime? CreatedDateTo { get; set; }

        public DateTime? CreatedDateFrom { get; set; }

        public int? CurrentInvoices { get; set; }

        public int? LateInvoices { get; set; }
    }
}
