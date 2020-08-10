using System.Collections.Generic;

namespace Core.Data.Dto {
    public class CompanyExportSettingsDto {
        public long Id { get; set; }

        public long? CompanyId { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public int Sort { get; set; }

        public bool IncludeAllCustomers { get; set; }

        public string DefaultValueIfEmpty { get; set; }

        public ICollection<CompanyExportSettingsFieldDto> Fields { get; set; }
    }

    public class CompanyExportSettingsFieldDto {
        public long Id { get; set; }

        public long? ExportSettingsId { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public int Sort { get; set; }

        public bool IsActive { get; set; }

        public bool IsEditable { get; set; }
    }
}
