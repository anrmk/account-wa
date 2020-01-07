using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Entities {
    [Table(name: "CompanyAddresses")]
    public class CompanyAddressEntity: EntityBase<long> {
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }

        public override string ToString() {
            return $"{Address}, {City}, {State}, {ZipCode}, {Country}".Trim(',').Trim();
        }
    }
}
