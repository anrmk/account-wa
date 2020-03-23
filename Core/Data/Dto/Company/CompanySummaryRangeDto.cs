namespace Core.Data.Dto {
    public class CompanySummaryRangeDto {
        public long Id { get; set; }
        public long CompanyId { get; set; }
        public decimal From { get; set; }
        public decimal To { get; set; }
    }
}
