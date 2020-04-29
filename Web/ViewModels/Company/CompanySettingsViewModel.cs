using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels {
    public class CompanySettingsViewModel {
        public long Id { get; set; }

        //public long? CompanyId { get; set; }

        [Display(Name = "Round Values")]
        public int RoundType { get; set; }

        [Display(Name = "Save Credit Value")]
        public bool SaveCreditValues { get; set; }
    }
}
