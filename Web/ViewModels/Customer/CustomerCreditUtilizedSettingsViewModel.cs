using System;
using System.ComponentModel.DataAnnotations;

using Core.Data.Enum;
using Core.Extension;

namespace Web.ViewModels {
    public class CustomerCreditUtilizedSettingsViewModel {
        public long Id { get; set; }

        [Required]
        [Display(Name = "Company")]
        public long CompanyId { get; set; }

        [Required]
        [Display(Name = "Round type")]
        public RoundType RoundType { get; set; }

        public string RoundName => EnumExtension.GetDescription(RoundType);

        [Required]
        [Display(Name = "Date")]
        public DateTime Date { get; set; }
    }
}
