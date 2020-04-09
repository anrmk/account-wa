using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Entities {
    [Table(name: "SavedReportFiles")]
    public class SavedReportFileEntity: EntityBase<long> {
        public string Name { get; set; }

        public byte[] File { get; set; }

        [ForeignKey("Report")]
        [Column("SavedReport_Id")]
        public long? ReportId { get; set; }
        public SavedReportEntity Report { get; set; }
    }
}
