using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Data.Dto {
    public class ReportFilterDto: PagerFilterDto {
        public long CompanyId { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public DateTime? FilterDate { get; set; }
    }
}
