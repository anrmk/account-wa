using System;

namespace Core.Data.Dto {
    public class CustomerRecheckDto {
        public long Id { get; set; }

        public long? CustomerId { get; set; }
        public string CustomerName { get; set; }

        public DateTime ReceivedDate { get; set; }
        public DateTime ReportDate { get; set; }

        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}