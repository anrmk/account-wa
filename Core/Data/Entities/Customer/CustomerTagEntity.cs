using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Entities {
    [Table(name: "CustomerTags")]
    public class CustomerTagEntity: EntityBase<long> {
        public string Name { get; set; }

        //public virtual ICollection<CustomerTagLinkEntity> CustomerTagLinks { get; set; }
    }
}
