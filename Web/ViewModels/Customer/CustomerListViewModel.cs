using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels {
    public class CustomerListViewModel {
        public long Id { get; set; }
        public string No { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Terms { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        public string CreditLimit { get; set; }
        public string CreditUtilized { get; set; }

        public long CompanyId { get; set; }
        public string Company { get; set; }

        public long TypeId { get; set; }
        public string Type { get; set; }

        public virtual ICollection<CustomerTagViewModel> Tags { get; set; }

        [Display(Name = "Is active")]
        public bool IsActive { get; set; }

        [Display(Name = "Total Invoices")]
        public int TotalInvoices { get; set; }

        [Display(Name = "Unaid Invoices")]
        public int UnpaidInvoices { get; set; }

        public int Recheck { get; set; }

        public DateTime ActivityDate { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}
