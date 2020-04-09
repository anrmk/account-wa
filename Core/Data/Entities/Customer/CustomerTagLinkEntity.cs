using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Entities {
    [Table(name: "CustomerTagLinks")]
    public class CustomerTagLinkEntity: EntityBase<long> {
        [ForeignKey("Customer")]
        [Column("Customer_Id")]
        public long? CustomerId { get; set; }
        public CustomerEntity Customer { get; set; }

        [ForeignKey("CustomerTag")]
        [Column("CustomerTag_Id")]
        public long? TagId { get; set; }
        public CustomerTagEntity Tag { get; set; }
    }
}
