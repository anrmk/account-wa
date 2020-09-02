using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Core.Extension;

namespace Web.ViewModels {
    public class InvoiceConstructorViewModel {
        public long Id { get; set; }

        [Required]
        [Display(Name = "Company")]
        public long CompanyId { get; set; }

        [Required]
        [Display(Name = "Date")]
        public DateTime Date { get; set; }

        [NotMapped]
        public DateTime DateFrom => Date.FirstDayOfMonth().AddDays(2);

        [NotMapped]
        public DateTime DateTo => Date.LastDayOfMonth();

        [Required]
        [Display(Name = "Search criterias")]
        public long SearchCriteriaId { get; set; }

        [Required]
        [Display(Name = "Summary range")]
        public long SummaryRangeId { get; set; }

        [Required]
        public List<long> Customers { get; set; }

        [Display(Name = "Select TOP record")]
        public int Count { get; set; }

        [Display(Name = "Total amount")]
        public decimal TotalAmount { get; set; }
    }
}
