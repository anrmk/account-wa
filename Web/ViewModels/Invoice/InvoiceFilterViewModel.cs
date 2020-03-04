using System;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels {
    public class InvoiceFilterViewModel: FilterViewModel {
        [Display(Name = "Company")]
        public long? CompanyId { get; set; }

        [Display(Name = "Period Date")]
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }

        [Display(Name = "Number of periods")]
        public int NumberOfPeriods { get; set; }

        [Display(Name = "Random sort")]
        public bool RandomSort { get; set; } = false;

        [Display(Name = "Period From")]
        public int? From { get; set; }

        [Display(Name = "Period To")]
        public int? To { get; set; }
    }
}
