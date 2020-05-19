using System;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels {
    public class PaymentFilterViewModel: PagerFilterViewModel {
        [Display(Name = "Company")]
        public long? CompanyId { get; set; }

        [Display(Name = "Date From")]
        [DataType(DataType.Date)]
        public DateTime? DateFrom { get; set; }

        [Display(Name = "Date To")]
        [DataType(DataType.Date)]
        public DateTime? DateTo { get; set; }

        [Display(Name = "Random sort")]
        public bool RandomSort { get; set; } = false;

        [Display(Name = "Created Date From")]
        [DataType(DataType.Date)]
        public DateTime? CreatedDateFrom { get; set; }

        [Display(Name = "Created Date To")]
        [DataType(DataType.Date)]
        public DateTime? CreatedDateTo { get; set; }
    }
}
