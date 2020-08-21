using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Entities {
    [Table(name: "SavedReportFields")]
    public class SavedReportFactFieldEntity: EntityBase<long> {
        public string Name { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Amount { get; set; }

        public int Count { get; set; }
   
        [ForeignKey("Report")]
        [Column("SavedReport_Id")]
        public long? ReportId { get; set; }
        public SavedReportFactEntity Report { get; set; }
    }
}
