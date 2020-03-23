using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Entities {
    [Table(name: "CompanyExportSettings")]
    public class CompanyExportSettingsEntity: EntityBase<long> {
        [Column("Company_Id")]
        public long? CompanyId { get; set; }
        public virtual CompanyEntity Company { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public int Sort { get; set; } = 0;

        public bool IncludeAllCustomers { get; set; }

        public virtual ICollection<CompanyExportSettingsFieldEntity> Fields { get; set; }
    }

    [Table(name: "CompanyExportSettingsFields")]

    public class CompanyExportSettingsFieldEntity: EntityBase<long> {
        [ForeignKey("ExportSettings")]
        [Column("CompanyExportSettings_Id", Order = 0)]
        public long? ExportSettingsId { get; set; }

        public virtual CompanyExportSettingsEntity ExportSettings { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Value { get; set; }

        public int Sort { get; set; } = 0;

        public bool IsActive { get; set; }

        public bool IsEditable { get; set; }
    }
}
