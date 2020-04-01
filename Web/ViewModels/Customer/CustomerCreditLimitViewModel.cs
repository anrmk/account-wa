using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.ViewModels {
    public class CustomerCreditLimitViewModel {
        public long Id { get; set; }

        [Required]
        public long? CustomerId { get; set; }

        [Display(Name = "Credit Limit")]
        [Column(TypeName = "decimal(18, 2)")]
        [Required]
        public decimal Value { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }
    }
}
