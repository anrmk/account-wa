using System;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels {
    public class CustomerViewModel {
        public long Id { get; set; }
        [Required]
        public string AccountNumber { get; set; }
        [Required]
        public string BusinessName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        [Required]
        public int Terms { get; set; }
        [Required]
        public DateTime DateOpen { get; set; }
    }
}
