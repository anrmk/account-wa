using System;

namespace Core.Data.Dto {
    public class ReportDataDto {
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
