using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Core.Data.Enum;

namespace Web.ViewModels {
    public class ReportFilterViewModel: PagerFilterViewModel {
        [Required]
        [Display(Name = "Company")]
        public long CompanyId { get; set; }

        [Required]
        [Display(Name = "Period")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now;

        public DateTime? DateFrom { get; set; }

        [Required]
        [Display(Name = "Number of periods")]
        [Range(1, 10)]
        public int NumberOfPeriods { get; set; } = 4;

        public RoundType RoundType { get; set; }

        //[Display(Name = "Customers")]
        public virtual List<long> CreditUtilizeds { get; set; }
    }
}
