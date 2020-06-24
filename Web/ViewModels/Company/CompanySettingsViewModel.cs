using System.ComponentModel.DataAnnotations;

using Core.Data.Enum;
using Core.Extension;

namespace Web.ViewModels {
    public class CompanySettingsViewModel {
        public long Id { get; set; }

        [Display(Name = "Round Values")]
        public RoundType RoundType { get; set; }

        public string RoundName => EnumExtension.GetDescription(RoundType);

        [Display(Name = "Save Credit Value")]
        public bool SaveCreditValues { get; set; }

        [Display(Name = "Customer account number template (regex)")]
        [MaxLength(64)]
        public string AccountNumberTemplate { get; set; }
    }
}
