using System;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels {
    public class InvoiceFilterViewModel: SearchViewModel {
        [Display(Name = "Company")]
        public long? CompanyId { get; set; }

        [Display(Name = "Period")]
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }

        [Display(Name = "Days per period")]
        public int DaysPerPeriod { get; set; } = 30;

        [Display(Name = "Random Sort")]
        public bool RandomSort { get; set; } = false;
    }
}
