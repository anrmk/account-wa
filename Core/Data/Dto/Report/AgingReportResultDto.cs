using System.Collections.Generic;

namespace Core.Data.Dto {
    public class AgingReportResultDto: ReportResultDto {
        public new List<AgingReportRowDto> Rows { get; set; }
        public new List<AgingReportColsDto> Cols { get; set; }

        public string CompanyName { get; set; }
        public int NumberOfPeriods { get; set; } //Количество периодово

        public Dictionary<string, int> CustomerTotal { get; set; }
        public Dictionary<string, int> CustomerTypes { get; set; }
        public Dictionary<string, AgingReportBalanceDto> BalanceTotal { get; set; }
        public Dictionary<string, decimal> DoubleDebt { get; set; }
    }

    //  Rows
    public class AgingReportRowDto: ReportRowDto {
        public string AccountNo { get; set; }
        public string CustomerName { get; set; }

        public CustomerDto Customer { get; set; }
    }

    //  Cols
    public class AgingReportColsDto {
        public int? From { get; set; }
        public int? To { get; set; }
        public string Name { get; set; }
    }

    //  Balace
    public class AgingReportBalanceDto {
        public decimal Sum { get; set; }
        public int Count { get; set; }
    }
}
