using System;
using System.Collections.Generic;

using AutoMapper.Configuration.Annotations;

namespace Core.Data.Dto {
    public class CompanyDto {
        public long Id { get; set; }

        #region GENERAL
        public string No { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public decimal TaxRate { get; set; }
        #endregion

        public long? AddressId { get; set; }
        public virtual CompanyAddressDto Address { get; set; }

        public virtual ICollection<CustomerDto> Customers { get; set; }

        [Ignore]
        public virtual ICollection<CompanySummaryRangeDto> SummaryRange { get; set; }

        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
