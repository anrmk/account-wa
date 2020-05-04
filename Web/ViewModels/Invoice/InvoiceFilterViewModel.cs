using System;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

namespace Web.ViewModels {
    public class InvoiceFilterViewModel: PagerFilterViewModel {
        [Display(Name = "Company")]
        public long? CompanyId { get; set; }

        [Display(Name = "Customer")]
        public long? CustomerId { get; set; }

        [Display(Name = "Period Date")]
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }

        [Display(Name = "Number of periods")]
        public int? NumberOfPeriods { get; set; }

        [Display(Name = "Random sort")]
        public bool RandomSort { get; set; } = false;

        [Display(Name = "Debtor with more than one invoices only")]
        [FromQuery(Name = "moreThanOne")]
        public bool MoreThanOne { get; set; } = false;

        [FromQuery(Name = "periods")]
        public string Periods { get; set; } = "";

        [Display(Name = "Customer type")]
        public long? TypeId { get; set; }

        [Display(Name = "Date From")]
        [DataType(DataType.Date)]
        public DateTime? DateFrom { get; set; }

        [Display(Name = "Date To")]
        [DataType(DataType.Date)]
        public DateTime? DateTo { get; set; }
    }
}
