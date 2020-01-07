using System;

namespace Core.Data.Dto {
    public class CompanyDtoList {
        public long Id { get; set; }
        public string AccountNumber { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }

    public class CompanyDto {
        public long Id { get; set; }
        public string AccountNumber { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }

        #region Address
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        #endregion

        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
        public string UpdatedBy { get; set; } = "system";
    }
}
