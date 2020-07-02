using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Entities {
    [Table(name: "CompanyRestrictedWords")]
    public class CompanyRestrictedWordEntity: EntityBase<long> {
        [ForeignKey("Company")]
        [Column("Company_Id")]
        public long? CompanyId { get; set; }
        public virtual CompanyEntity Company { get; set; }

        [ForeignKey("RestrictedWord")]
        [Column("RestrictedWord_Id")]
        public long? RestrictedWordId { get; set; }
        public virtual SettingsRestrictedWordEntity RestrictedWord { get; set; }
    }
}
