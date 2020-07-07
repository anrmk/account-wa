using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.ViewModels {
    public class CreditUtilizedReportViewModel {
        public long Id { get; set; }
        public string Name { get; set; }
        public string AccountNumber { get; set; }
        public string CreateDate { get; set; }
        public string Value { get; set; }
    }
}
