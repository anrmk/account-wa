﻿using System;
using System.Collections.Generic;

namespace Core.Data.Dto {
    public class SavedReportDto {
        public long Id { get; set; }
        public string Name { get; set; }
        public long? CompanyId { get; set; }
        public DateTime Date { get; set; }
        public int NumberOfPeriods { get; set; }
        public bool IsPublished { get; set; }

        public Guid ApplicationUserId { get; set; }

        public virtual ICollection<SavedReportFieldDto> Fields { get; set; }

        public virtual ICollection<SavedReportFileDto> Files { get; set; }

        public virtual IList<long> ExportSettings { get; set; }
    }
}
