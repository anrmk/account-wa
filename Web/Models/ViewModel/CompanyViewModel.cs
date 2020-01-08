﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels {
    public class CompanyViewModelList {
        public long Id { get; set; }
        public string AccountNumber { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }

    public class CompanyViewModel {
        public long Id { get; set; }

        [Required]
        [MaxLength(6)]
        public string AccountNumber { get; set; }

        [Required]
        [MaxLength(256)]
        public string Name { get; set; }
        public string Description { get; set; }
        public string PhoneNumber { get; set; }

        #region Address
        public long? AddressId { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        #endregion
    }
}
