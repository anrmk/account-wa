using System;
using System.Collections.Generic;
using System.Linq;

using AutoMapper.Configuration.Annotations;

using Core.Data.Dto.Nsi;

using Newtonsoft.Json;

namespace Core.Data.Dto {
    public class CustomerDto {
        public long Id { get; set; }

        [JsonProperty("Account Number")]
        public string No { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string PhoneNumber { get; set; }
        public string Terms { get; set; }

        public decimal? CreditLimit {
            get {
                if(CreditLimits != null && CreditLimits.Count > 0) {
                    return CreditLimits.OrderByDescending(x => x.CreatedDate)
                        .Select(x => x.Value).FirstOrDefault();
                }
                return (decimal?)null;
            }
        }

        public decimal? CreditUtilized {
            get {
                if(CreditUtilizeds != null && CreditUtilizeds.Count > 0) {
                    return CreditUtilizeds.OrderByDescending(x => x.CreatedDate)
                        .Select(x => x.Value).FirstOrDefault();
                }
                return (decimal?)null;
            }
        }

        public long? AddressId { get; set; }
        public CustomerAddressDto Address { get; set; }

        public long? CompanyId { get; set; }
        public CompanyDto Company { get; set; }

        public long? TypeId { get; set; }
        public NsiDto Type { get; set; }

        public virtual ICollection<InvoiceDto> Invoices { get; set; }

        public virtual ICollection<CustomerCreditUtilizedDto> CreditUtilizeds { get; set; }

        public virtual ICollection<CustomerCreditLimitDto> CreditLimits { get; set; }

        public virtual ICollection<CustomerActivityDto> Activities { get; set; }

        public virtual ICollection<long?> TagsIds { get; set; }
        public virtual ICollection<CustomerTagDto> Tags { get; set; }

        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

        #region EXTENDED
        public bool IsActive { get; set; }

        //only for bulk customers!!!
        [Ignore]
        public int TotalInvoices { get; set; }

        [Ignore]
        public int PaidInvoices { get; set; }

        [Ignore]
        public int Recheck { get; set; }
        #endregion
    }
}
