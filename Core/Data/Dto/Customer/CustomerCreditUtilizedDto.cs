﻿using System;

namespace Core.Data.Dto {
    public class CustomerCreditUtilizedDto {
        public long Id { get; set; }

        public long? CustomerId { get; set; }
        public virtual CustomerDto Customer { get; set; }

        public decimal? Value { get; set; }

        public bool IsIgnored { get; set; }

        public DateTime CreatedDate { get; set; }

        public decimal? NewValue { get; set; }
        public bool IsNew { get; set; }
        public DateTime NewCreatedDate { get; set; }
    }
}
