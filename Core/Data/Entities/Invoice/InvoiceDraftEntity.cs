using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Entities {
    [Table(name: "InvoiceDrafts")]
    public class InvoiceDraftEntity: EntityBase<long> {
        [Required]
        [MaxLength(16)]
        public string No { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Subtotal { get; set; }

        [Range(0, 100)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TaxRate { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [Column("Constructor_Id")]
        public long? ConstructorId { get; set; }

        [Column("Company_Id")]
        public long? CompanyId { get; set; }

        [Column("Customer_Id")]
        public long? CustomerId { get; set; }
    }
}
