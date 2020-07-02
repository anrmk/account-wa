using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels {
    public class SettingsRestrictedWordViewModel {
        public long Id { get; set; }

        [Required]
        [MaxLength(32)]
        public string Name { get; set; }

        [Display(Name = "Companies")]
        public virtual ICollection<long?> CompanyIds { get; set; }
    }
}
