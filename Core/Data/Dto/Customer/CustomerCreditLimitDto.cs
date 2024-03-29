﻿using System;

namespace Core.Data.Dto {
    public class CustomerCreditLimitDto {
        public long Id { get; set; }

        public long? CustomerId { get; set; }
        public virtual CustomerDto Customer { get; set; }

        public decimal? Value { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
