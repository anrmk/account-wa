using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels {
    public class ReportViewModel {
        [Display(Name = "Company")]
        public long? CompanyId { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Period")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now;

        [Display(Name = "Days per period")]
        public int DaysPerPeriod { get; set; } = 30;

        [Display(Name = "Number of periods")]
        public int NumberOfPeriod { get; set; } = 4;
    }

    public class AgingReportViewModel<T> {
        [Display(Name = "Company")]
        public long CompanyId { get; set; }

        public string CompanyName { get; set; }

        // [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Period")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now;

        [Display(Name = "Days per period")]
        public int DaysPerPeriod { get; set; } = 30;

        [Display(Name = "Number of periods")]
        public int NumberOfPeriod { get; set; } = 4;

        public List<string> NamesOfPeriod { get; set; }

        public List<T> Datas { get; set; }

        public Dictionary<long, T> DataForm { get; set; }
    }

    public class AgingReportDataViewModel {
        public long Id { get; set; }

        public long CustomerId { get; set; }
        public string AccountNumber { get; set; }
        public string CustomerName { get; set; }

        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
        public decimal? PayAmount { get; set; }
        public DateTime? PayDate { get; set; }
        public int? DiffDate { get; set; }

        public Dictionary<string, decimal> Aging { get; set; }
    }
}
