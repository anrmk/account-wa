using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Entities {
    [Table(name: "SavedReportFields")]
    public class SavedReportFieldEntity: EntityBase<long> {
        public string Name { get; set; }

        public string Value { get; set; }

        [ForeignKey("Report")]
        [Column("SavedReport_Id")]
        public long? ReportId { get; set; }
        public SavedReportEntity Report { get; set; }

        public string Code { get; set; }
    }
}
