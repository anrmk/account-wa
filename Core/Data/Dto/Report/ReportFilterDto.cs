using System;

namespace Core.Data.Dto {
    public class ReportFilterDto: PagerFilterDto {
        public Guid? ApplicationUserId { get; set; }
        public long CompanyId { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public DateTime? FilterDate { get; set; }
    }
}
