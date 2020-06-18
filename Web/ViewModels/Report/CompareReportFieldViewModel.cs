using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels {
    public class CompareReportViewModel {
        [Display(Name = "Customers")]
        public ICollection<CompareReportFieldViewModel> Customers { get; set; }

        [Display(Name = "Customer Types")]
        public ICollection<CompareReportFieldViewModel> CustomerTypes { get; set; }

        [Display(Name = "Balance")]
        public ICollection<CompareReportFieldViewModel> Balance { get; set; }

        [Display(Name = "Credit Utilized")]
        public ICollection<CompareReportFieldViewModel> CreditUtilized { get; set; }
    }

    public class CompareReportFieldViewModel {
        public string Name { get; set; }
        public string SavedValue { get; set; }
        public string ReportValue { get; set; }
        public bool Status { get; set; }
    }
}
