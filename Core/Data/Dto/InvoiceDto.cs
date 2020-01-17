﻿using System;
using System.Collections.Generic;

using Core.Extension;

namespace Core.Data.Dto {
    public class InvoiceDto {
        public long Id { get; set; }
        public string No { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TaxRate { get; set; }

        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }

        public List<PaymentDto> Payment { get; set; }
        public decimal PaymentAmount => PaymentExtension.TotalAmount(Payment);
        public DateTime? PaymentDate => PaymentExtension.LastPaymentDate(Payment);

        public long? CompanyId { get; set; }
        public CompanyDto Company { get; set; }

        public long? CustomerId { get; set; }
        public CustomerDto Customer { get; set; }
    }
}
