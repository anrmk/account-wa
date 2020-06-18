using System;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels {
    public class CustomerActivityViewModel {
        public long Id { get; set; }

        [Display(Name = "Customer")]
        public long? CustomerId { get; set; }

        [Display(Name = "Record Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Is active")]
        public bool IsActive { get; set; }
    }
}
