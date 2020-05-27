using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Web.ViewModels {
    public class ReportSearchCriteriaViewModel {
        public long Id { get; set; }

        public int Sort { get; set; } = 0;

        [Display(Name = "Tags")]
        public List<long> TagsIds { get; set; }

        [Display(Name = "Types")]
        public List<long> TypeIds { get; set; }

        [Display(Name = "Recheck")]
        public List<int> Recheck { get; set; }

        [Display(Name = "Current Invoices")]
        public int? CurrentInvoices { get; set; }

        [Display(Name = "Late Invoices")]
        public int? LateInvoices { get; set; }

        [Display(Name = "Date From")]
        [DataType(DataType.Date)]
        public DateTime? DateFrom { get; set; }

        [Display(Name = "Date To")]
        [DataType(DataType.Date)]
        public DateTime? DateTo { get; set; }

        [Display(Name = "Created Date From")]
        [DataType(DataType.Date)]
        public DateTime? CreatedDateFrom { get; set; }

        [Display(Name = "Created Date To")]
        [DataType(DataType.Date)]
        public DateTime? CreatedDateTo { get; set; }

        [Display(Name = "Random sort")]
        public bool RandomSort { get; set; }

        [Display(Name = "Only new customer")]
        public bool OnlyNewCustomer { get; set; }

        [Display(Name = "Exclude new customer")]
        public bool ExcludeNewCustomer { get; set; }

        [Display(Name = "Select top records")]
        public int SelectTop { get; set; }
    }
}
