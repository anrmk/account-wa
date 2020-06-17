using System;
using System.ComponentModel.DataAnnotations;
using Core.Data.Enum;
using Core.Extension;

namespace Web.ViewModels {
    public class CustomerCreditUtilizedSettingsViewModel {
        public long Id { get; set; }

        [Required]
        public long CompanyId { get; set; }

        [Required]
        public RoundType RoundType { get; set; }

        public string RoundName => EnumExtension.GetDescription(RoundType);

        [Required]
        public DateTime Date { get; set; }
    }
}
