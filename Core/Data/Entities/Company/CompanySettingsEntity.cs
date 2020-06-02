using System.ComponentModel.DataAnnotations.Schema;

using Core.Data.Enum;

namespace Core.Data.Entities {
    [Table(name: "CompanySettings")]
    public class CompanySettingsEntity: EntityBase<long> {
        public RoundType RoundType { get; set; } = 0;

        public bool SaveCreditValues { get; set; }
    }
}
