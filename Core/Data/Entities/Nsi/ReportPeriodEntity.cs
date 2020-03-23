using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Entities.Nsi {
    [Table(name: "nsi.ReportPeriods")]
    public class ReportPeriodEntity: NsiEntity<long> {
        [Required]
        public int From { get; set; }

        [Required]
        public int To { get; set; }
    }
}
