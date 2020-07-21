using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Entities {
    [Table(name: "SavedReportPlanFields")]
    public class SavedReportPlanFieldEntity: EntityBase<long> {
        public string Name { get; set; }

        public string Value { get; set; }

        [ForeignKey("Report")]
        [Column("SavedReport_Id")]
        public long? ReportId { get; set; }
        public SavedReportPlanEntity Report { get; set; }

        public string Code { get; set; }
    }
}
