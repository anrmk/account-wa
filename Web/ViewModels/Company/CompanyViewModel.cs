﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Core.Data.Enum;

namespace Web.ViewModels {
    public class CompanyListViewModel {
        public long Id { get; set; }
        public string No { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public double TaxRate { get; set; }

        public string Address { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        public int TotalCustomers { get; set; }
    }

    public class CompanyViewModel {
        public long Id { get; set; }

        #region GENERAL
        [Required]
        [MaxLength(8)]
        public string No { get; set; }

        [Required]
        [MaxLength(256)]
        public string Name { get; set; }

        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Display(Name = "Tax Rate")]
        [Range(0, 100)]
        public double TaxRate { get; set; }
        #endregion

        #region ADDRESS
        public long AddressId { get; set; }

        [MaxLength(60)]
        public string Address { get; set; }

        [MaxLength(60)]
        public string Address2 { get; set; }

        [MaxLength(60)]
        public string City { get; set; }

        [MaxLength(60)]
        public string State { get; set; }

        [MaxLength(10)]
        public string ZipCode { get; set; }

        [MaxLength(60)]
        public string Country { get; set; }
        #endregion

        #region SETTINGS
        public long SettingsId { get; set; }

        [Display(Name = "Round Type")]
        public RoundType RoundType { get; set; }

        [Display(Name = "Save High Credit Utilized")]
        public bool SaveCreditValues { get; set; }

        [Display(Name = "Customer account number template (regex)")]
        [MaxLength(64)]
        public string AccountNumberTemplate { get; set; }
        #endregion

        public IList<long> Customers { get; set; }

        public IList<CompanyExportSettingsViewModel> ExportSettings { get; set; }
    }
}
