using System;

namespace Core.Data.Dto {
    public class CustomerImportCreditsDto {
        public string No { get; set; }

        //public string Name { get; set; }

        public decimal? CreditLimit { get; set; }

        public decimal? CreditUtilized { get; set; }

        public long? CompanyId { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
