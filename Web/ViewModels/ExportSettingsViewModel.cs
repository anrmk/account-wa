
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels {
    public class ExportSettingsViewModel {
        [Display(Name = "Company")]
        public long CompanyId { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Show blank invoices")]
        public bool ShowEmptyRows { get; set; }

        public virtual ICollection<ExportSettingsFieldValueViewModel> Fields { get; set; }
    }

    public class ExportSettingsFieldValueViewModel {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
