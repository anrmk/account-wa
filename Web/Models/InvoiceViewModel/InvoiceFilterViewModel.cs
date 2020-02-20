using System;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels {
    public class InvoiceFilterViewModel: SearchViewModel {
        [Display(Name = "Company")]
        public long? CompanyId { get; set; }

        //[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        //[DisplayFormat(DataFormatString = @"{0:MM\/dd\/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Period")]
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }

        [Display(Name = "Days per period")]
        public int DaysPerPeriod { get; set; } = 30;

        [Display(Name = "Number of periods")]
        public int NumberOfPeriods { get; set; }

        [Display(Name = "Random sort")]
        public bool RandomSort { get; set; } = false;
    }
}
