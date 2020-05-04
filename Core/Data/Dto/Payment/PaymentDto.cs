﻿using System;

namespace Core.Data.Dto {
    public class PaymentDto {
        public long Id { get; set; }
        public string No { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public long? InvoiceId { get; set; }
        public string InvoiceNo { get; set; }
        public long? CustomerId { get; set; }
        public string CompanyName { get; set; }
    }
}
