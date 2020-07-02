using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels {
    public class CustomerSettingsRestrictedWordViewModel {
        public long Id { get; set; }

        [Required]
        [MaxLength(32)]
        public string Name { get; set; }
    }
}
