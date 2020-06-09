using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Core.Data.Enum;

namespace Core.Data.Entities {
    [Table(name: "InvoiceConstructorSearches")]
    public class InvoiceConstructorSearchEntity: EntityBase<long> {
        [MaxLength(256)]
        public string Name { get; set; }
        public int Sort { get; set; } = 0;

        public string CustomerTags { get; set; }
        public string CustomerTypes { get; set; }
        public string CustomerRechecks { get; set; }

        public int? CurrentInvoices { get; set; }
        public int? LateInvoices { get; set; }

        public bool RandomSort { get; set; } = false;

        public CustomerGroupType Group { get; set; }
    }
}
