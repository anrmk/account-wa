using System;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels {
    public class CustomerRecheckViewModel {
        public long Id { get; set; }

        [Required]
        [Display(Name = "Customer")]
        public long? CustomerId { get; set; }

        public string CustomerName { get; set; }

        [Required]
        [Display(Name = "Received Date")]
        [DataType(DataType.Date)]
        public DateTime ReceivedDate { get; set; }
        
        [Required]
        [Display(Name = "Report Date")]
        [DataType(DataType.Date)]
        public DateTime ReportDate { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }
    }
}
