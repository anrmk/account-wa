using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.ViewModels {
    public class PaymentListViewModel {
        public long Id { get; set; }
        public string No { get; set; }
        public string InvoiceNo { get; set; }
        public long InvoiceId { get; set; }
        public decimal InvoiceAmount { get; set; }
        public decimal Amount { get; set; }
    }
}
