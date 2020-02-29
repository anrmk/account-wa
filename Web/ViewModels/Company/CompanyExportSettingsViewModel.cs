using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels {
    public class CompanyExportSettingsViewModel {
        public long Id { get; set; }

        [Display(Name = "Company")]
        public long CompanyId { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "File Title")]
        public string Title { get; set; }

        [Display(Name = "Show blank invoices")]
        public bool ShowEmptyRows { get; set; }

        [Display(Name = "Fields")]
        public List<CompanyExportSettingsFieldViewModel> Fields { get; set; }
    }

    public class CompanyExportSettingsFieldViewModel {
        public long Id { get; set; }

        public long ExportSettingsId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Value { get; set; }

        public int Sort { get; set; }

        [Display(Name = "Is active")]
        public bool IsActive { get; set; }

        [Display(Name = "Is editable")]
        public bool IsEditable { get; set; }
    }
}
