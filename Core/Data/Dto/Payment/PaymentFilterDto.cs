﻿using System;

namespace Core.Data.Dto {
    public class PaymentFilterDto: PagerFilterDto {
        public long? CompanyId { get; set; }

        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public DateTime? CreatedDateFrom { get; set; }
        public DateTime? CreatedDateTo { get; set; }
    }
}
