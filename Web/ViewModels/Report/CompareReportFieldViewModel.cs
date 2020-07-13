using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels {
    public class CompareReportViewModel {
        public long CompanyId { get; set; }
        public DateTime Date { get; set; }
        public int NumberOfPeriods { get; set; }

        [Display(Name = "Customers")]
        public ICollection<CompareReportFieldViewModel> Customers { get; set; }

        [Display(Name = "Customer Types")]
        public ICollection<CompareReportFieldViewModel> CustomerTypes { get; set; }

        [Display(Name = "Balance")]
        public ICollection<CompareReportFieldViewModel> Balance { get; set; }

        [Display(Name = "Credit Utilized")]
        public ICollection<CompareCreditsFieldViewModel> CreditUtilized { get; set; }

        public ICollection<CompareReportCreditUtilizedViewModel> CreditUtilizedList { get; set; }
    }

    public class CompareReportFieldViewModel {
        public string Name { get; set; }
        public string SavedValue { get; set; }
        public string ReportValue { get; set; }
        public bool Status { get; set; }
    }

    public class CompareCreditsFieldViewModel {
        public string Name { get; set; }
        public int CreateCount { get; set; }
        public int UpdateCount { get; set; }
        public int IgnoredCount { get; set; }
        public bool Status { get; set; }
    }

    public class CompareReportCreditUtilizedViewModel {
        public long Id { get; set; }

        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public decimal OldValue { get; set; }
        public DateTime? OldDate { get; set; }
        public decimal NewValue { get; set; }
        public bool IsIgnored { get; set; }

        public bool Status { get; set; }
    }
}
