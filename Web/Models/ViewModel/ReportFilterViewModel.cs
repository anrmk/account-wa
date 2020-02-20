using System;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels {
    public class ReportFilterViewModel {
        [Display(Name = "Company")]
        public long? CompanyId { get; set; }

        // [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Period")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now;

        [Display(Name = "Days per period")]
        public int DaysPerPeriod { get; set; } = 30;

        [Display(Name = "Number of periods")]
        public int NumberOfPeriods { get; set; } = 4;
    }
}
