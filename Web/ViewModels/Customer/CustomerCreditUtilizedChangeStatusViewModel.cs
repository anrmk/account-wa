using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels {
    public class CustomerCreditUtilizedChangeStatusViewModel {
        [Required]
        public virtual ICollection<CustomerCreditUtilizedViewModel> Credits { get; set; }

        public bool IsIgnored { get; set; }
    }
}
