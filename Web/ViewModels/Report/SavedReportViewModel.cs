using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels {
    public class SavedReportListViewModel {
        public long Id { get; set; }

        public string Name { get; set; }
        public long CompanyId { get; set; }

        public int Count { get; set; }
    }

    public class SavedReportViewModel {
        public long Id { get; set; }

        public string Name { get; set; }

        public long CompanyId { get; set; }

        [Display(Name = "Period")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Display(Name = "Number of periods")]
        [Range(1, 10)]
        public int NumberOfPeriods { get; set; }

        public IList<long> ExportSettings { get; set; }
    }
}
