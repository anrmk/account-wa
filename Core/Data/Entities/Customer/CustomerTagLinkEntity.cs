using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Entities {
    [Table(name: "CustomerTagLinks")]
    public class CustomerTagLinkEntity: EntityBase<long> {
        [ForeignKey("Customer")]
        [Column("Customer_Id")]
        public long? CustomerId { get; set; }
        public virtual CustomerEntity Customer { get; set; }

        [ForeignKey("CustomerTag")]
        [Column("CustomerTag_Id")]
        public long? TagId { get; set; }
        public virtual CustomerTagEntity Tag { get; set; }
    }
}
