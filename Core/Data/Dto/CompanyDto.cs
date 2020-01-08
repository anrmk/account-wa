using System;

namespace Core.Data.Dto {
    public class CompanyDto {
        public long? Id { get; set; }
        public string AccountNumber { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PhoneNumber { get; set; }

        public CompanyAddressDto Address { get; set; }

        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
