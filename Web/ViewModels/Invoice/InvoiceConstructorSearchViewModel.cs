using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Core.Data.Enum;
using Core.Extension;

namespace Web.ViewModels {
    public class InvoiceConstructorSearchViewModel {
        public long Id { get; set; }


        [MaxLength(256)]
        public string Name { get; set; }

        public int Sort { get; set; } = 0;

        [Display(Name = "Tags")]
        public List<long> TagsIds { get; set; }

        [Display(Name = "Types")]
        public List<long> TypeIds { get; set; }

        [Display(Name = "Recheck")]
        public List<int> Recheck { get; set; }

        [Display(Name = "Current Invoices")]
        public int? CurrentInvoices { get; set; }

        [Display(Name = "Late Invoices")]
        public int? LateInvoices { get; set; }

        [Display(Name = "Random sort")]
        public bool RandomSort { get; set; }

        [Display(Name = "Customer Group")]
        public CustomerGroupType Group { get; set; }

        public string GroupName => EnumExtension.GetDescription(Group);
    }
}
