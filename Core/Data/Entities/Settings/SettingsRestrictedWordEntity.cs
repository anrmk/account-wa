using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Entities {
    [Table(name: "SettingsRestrictedWords")]
    public class SettingsRestrictedWordEntity: EntityBase<long> {
        [Required]
        [MaxLength(32)]
        public string Name { get; set; }
    }
}
