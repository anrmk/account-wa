using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.ViewModels {
    public class CompanySummaryRangeViewModel {
        public long CompanyId { get; set; }

        [Display(Name = "From")]
        [Range(0, 999999)]
        public decimal From { get; set; }

        [Display(Name = "To")]
        [Range(0, 999999)]
        public decimal To { get; set; }
    }
}
