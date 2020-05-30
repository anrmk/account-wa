using System;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels {
    public class InvoiceConstructorViewModel {
        public long Id { get; set; }

        [Required]
        [Display(Name = "Company")]
        public long CompanyId { get; set; }

        [Required]
        [Display(Name = "Date")]
        public DateTime Date { get; set; }

        [Required]
        [Display(Name = "Search criterias")]
        public long SearchCriteriaId { get; set; }

        [Required]
        [Display(Name = "Summary range")]
        public long SummaryRangeId { get; set; }

        [Required]
        [Display(Name = "Select TOP record")]
        public int Count { get; set; }

        [Display(Name = "Total amount")]
        public decimal TotalAmount { get; set; }
    }
}
