using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels {
    public class SavedReportPlanViewModel {
        public long Id { get; set; }

        public string Name { get; set; }

        public long CompanyId { get; set; }

        [Display(Name = "Period")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Display(Name = "Number of periods")]
        [Range(1, 10)]
        public int NumberOfPeriods { get; set; }

        public bool IsPublished { get; set; }


        public List<SavedReportPlanFieldViewModel> Fields { get; set; }
    }
}
