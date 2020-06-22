﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.ViewModels {
    public class CustomerCreditUtilizedViewModel {
        public long Id { get; set; }

        [Required]
        [Display(Name = "Customer")]
        public long? CustomerId { get; set; }

        [Required]
        [Display(Name = "Credit Utilized")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Value { get; set; }

        [Display(Name ="Is Ignored")]
        public bool IsIgnored { get; set; }

        [Required]
        [Display(Name = "Record Date")]
        public DateTime CreatedDate { get; set; }
    }
}
