using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Entities {
    [Table(name: "CustomerTypes")]
    public class CustomerTypeEntity: EntityBase<long> {
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
