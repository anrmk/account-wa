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
    }
}
