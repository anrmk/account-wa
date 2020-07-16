using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.ViewModels {
    public class InvoiceImportCsvViewModel: ImportCsvViewModel {
        [Required]
        public long CompanyId { get; set; }
    }
}
