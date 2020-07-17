using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels {
    public class InvoiceImportCsvViewModel: ImportCsvViewModel {
        [Required]
        public long CompanyId { get; set; }
    }
}
