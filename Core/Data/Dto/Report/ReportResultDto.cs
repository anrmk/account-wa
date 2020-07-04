using System;
using System.Collections.Generic;

namespace Core.Data.Dto {
    public class ReportResultDto {
        public long CompanyId { get; set; }
        public DateTime Date { get; set; }

        public virtual List<ReportRowDto> Rows { get; set; }
        public virtual List<ReportColsDto> Cols { get; set; }
    }

    public class ReportRowDto {
        public Dictionary<string, decimal> Data { get; set; }
    }

    public class ReportColsDto {
        public string Name { get; set; }
    }
}
