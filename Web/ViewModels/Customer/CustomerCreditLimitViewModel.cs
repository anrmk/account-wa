using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.ViewModels {
    public class CustomerCreditLimitViewModel {
        public long Id { get; set; }

        [Required]
        [Display(Name = "Customer")]
        public long? CustomerId { get; set; }

        [Required]
        [Display(Name = "Credit Limit")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Value { get; set; }

        [Required]
        [Display(Name = "Record Date")]
        public DateTime CreatedDate { get; set; }
    }
}
