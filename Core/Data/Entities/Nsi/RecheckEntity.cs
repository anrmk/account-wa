using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Entities.Nsi {
    [Table(name: "nsi.Recheck")]
    public class RecheckEntity: NsiEntity<long> {
        [Required]
        [Column("Customer_Id")]
        public long? CustomerId { get; set; }

        public DateTime Date { get; set; }

        public DateTime ReportDate { get; set; }
    }
}
