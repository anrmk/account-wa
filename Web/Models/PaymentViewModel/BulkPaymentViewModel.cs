using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Core.Extension;

namespace Web.ViewModels {
    public class BulkPaymentViewModel {
        public string Header { get; set; }

        [Display(Name = "Date from")]
        [DataType(DataType.Date)]
        public DateTime DateFrom { get; set; } = DateTime.Now;

        [Display(Name = "Date to")]
        [DataType(DataType.Date)]
        public DateTime DateTo { get; set; } = DateTime.Now.AddDays(30);

        [Display(Name = "Company")]
        public long? CompanyId { get; set; }

        [Display(Name = "Invoices")]
        public List<long> Invoices { get; set; }


        [Display(Name = "Date from")]
        [DataType(DataType.Date)]
        public DateTime PaymentDateFrom { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

        [Display(Name = "Date to")]
        [DataType(DataType.Date)]
        public DateTime PaymentDateTo { get; set; } = DateTime.Now.LastDayOfMonth();

        [Display(Name = "Payments")]
        public virtual List<PaymentViewModel> Payments { get; set; }
    }
}
