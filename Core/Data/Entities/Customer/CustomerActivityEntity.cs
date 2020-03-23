using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Entities {
    [Table(name: "CustomerActivities")]
    public class CustomerActivityEntity: EntityBase<long> {
        [ForeignKey("Customer")]
        [Column("Customer_Id")]
        public long? CustomerId { get; set; }
        public virtual CustomerEntity Customer { get; set; }

        [ScaffoldColumn(false)]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; }
    }
}
