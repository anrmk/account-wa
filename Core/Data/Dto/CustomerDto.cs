using System;

namespace Core.Data.Dto {
    public class CustomerDto {
        public long? Id { get; set; }
        public string AccountNumber { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PhoneNumber { get; set; }
        public string Terms { get; set; }

        public double CreditLimit { get; set; }
        public double CreditUtilized { get; set; }

        public CustomerAddressDto Address { get; set; }

        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
