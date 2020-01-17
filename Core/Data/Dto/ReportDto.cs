using System;
using System.Collections.Generic;

namespace Core.Data.Dto {
    public class ReportDto<T> {
        public long CompanyId { get; set; }
        public DateTime Date { get; set; }
        public int DaysPerAgingPeriod { get; set; } = 30;
        public int NumberOfPeriod { get; set; } = 4;

        public List<T> Datas { get; set; }

        public List<T> AddData(T data) {
            Datas.Add(data);
            return Datas;
        }
    }

    public class AgingReportDataDto {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string AccountNumber { get; set; }
        public string No { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
        public decimal? PayAmount { get; set; }
        public DateTime? PayDate { get; set; }
        public int? DiffDate { get; set; }
    }
}
