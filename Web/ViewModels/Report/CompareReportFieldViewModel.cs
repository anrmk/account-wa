using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.ViewModels {
    public class CompareReportViewModel {
        public ICollection<CompareReportFieldViewModel> Customers { get; set; }
        public ICollection<CompareReportFieldViewModel> Balance { get; set; }
        public ICollection<CompareReportFieldViewModel> CustomerTypes { get; set; }
        public ICollection<CompareReportFieldViewModel> CreditUtilized { get; set; }
    }

    public class CompareReportFieldViewModel {
        public string Name { get; set; }
        public string SavedValue { get; set; }
        public string ReportValue { get; set; }
        public bool Status { get; set; }
    }
}
