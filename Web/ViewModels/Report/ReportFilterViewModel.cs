using System;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels {
    public class ReportFilterViewModel {
        [Required]
        [Display(Name = "Company")]
        public long CompanyId { get; set; }

        [Display(Name = "Period")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now;

        [Display(Name = "Number of periods")]
        [Range(1, 10)]
        public int NumberOfPeriods { get; set; } = 4;
    }
}
