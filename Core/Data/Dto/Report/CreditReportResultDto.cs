using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Data.Dto {
    public class CreditReportResultDto: ReportResultDto {
        public new List<CreditReportRowDto> Rows { get; set; }
        public DateTime DateFrom { get; set; }
        public Dictionary<string, List<string>> Filter { get; set; }
    }

    public class CreditReportRowDto {
        public long Id { get; set; }
        public Dictionary<string, string> Data { get; set; }
    }
}
