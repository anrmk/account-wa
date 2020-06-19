using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels {
    public class CompareReportViewModel {
        public DateTime Date { get; set; }

        [Display(Name = "Customers")]
        public ICollection<CompareReportFieldViewModel> Customers { get; set; }

        [Display(Name = "Customer Types")]
        public ICollection<CompareReportFieldViewModel> CustomerTypes { get; set; }

        [Display(Name = "Balance")]
        public ICollection<CompareReportFieldViewModel> Balance { get; set; }

        [Display(Name = "Credit Utilized")]
        public ICollection<CompareReportFieldViewModel> CreditUtilized { get; set; }

        public ICollection<CompareReportCreditUtilizedViewModel> CreditUtilizedList { get; set; }
    }

    public class CompareReportFieldViewModel {
        public string Name { get; set; }
        public string SavedValue { get; set; }
        public string ReportValue { get; set; }
        public bool Status { get; set; }
    }

    public class CompareReportCreditUtilizedViewModel {
        public string No { get; set; }
        public string Name { get; set; }
        public decimal OldValue { get; set; }
        public decimal NewValue { get; set; }
        public bool Status { get; set; }
    }
}
