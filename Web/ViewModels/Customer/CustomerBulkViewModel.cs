using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels {
    //Customer Credit Limits & Credit Utilized Import Fomr
    public class CustomerCreditsBulkViewModel: CustomerBulkViewModel {
        [Required]
        public DateTime CreatedDate { get; set; }
    }

    //Customer Import Form
    public class CustomerBulkViewModel {
        [Required]
        [Display(Name = "Company*")]
        public long? CompanyId { get; set; }

        public List<CustomerColViewModel> Columns { get; set; }

        //import header
        public List<string> HeadRow { get; set; }

        //import rows
        public List<CustomerRowViewModel[]> Rows { get; set; }

        [Required]
        public int[] CheckedRecords { get; set; }
    }

    public class CustomerColViewModel {
        public int Index { get; set; }
        public string Name { get; set; }
    }

    public class CustomerRowViewModel {
        public int Index { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
