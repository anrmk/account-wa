using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.ViewModels {
    public class CustomerCreditUtilizedViewModel {
        public long Id { get; set; }

        public long? CustomerId { get; set; }

        [Display(Name = "Credit Utilized")]
        [Column(TypeName = "decimal(18, 2)")]
        [Required]
        public decimal Value { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }
    }
}
