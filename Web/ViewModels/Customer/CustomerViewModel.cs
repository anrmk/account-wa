using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Web.ViewModels {
    public class CustomerViewModel {
        public long Id { get; set; }

        [Required]
        [Display(Name = "Account Number")]
        [MaxLength(16)]
        [SpecialName()]
        public string No { get; set; }

        [Required]
        [Display(Name = "Business Name")]
        [MaxLength(256)]
        [SpecialName()]
        public string Name { get; set; }

        [MaxLength(2048)]
        public string Description { get; set; }

        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        [SpecialName()]
        public string PhoneNumber { get; set; }

        [SpecialName()]
        public string Terms { get; set; }

        //[Display(Name = "Credit Limit")]
        //[Column(TypeName = "decimal(18, 2)")]
        //[SpecialName()]
        //public double CreditLimit { get; set; }

        //[Display(Name = "Credit Utilized")]
        //[Column(TypeName = "decimal(18, 2)")]
        //[SpecialName()]
        //public double CreditUtilized { get; set; }

        [Display(Name = "Company")]
        public long? CompanyId { get; set; }

        [SpecialName()]
        [Display(Name = "Type")]
        public long? TypeId { get; set; }

        public virtual ICollection<CustomerActivityViewModel> Activities { get; set; }

        [Display(Name = "Created Date")]
        [SpecialName()]
        public DateTime CreatedDate { get; set; }

        #region Address
        public long? AddressId { get; set; }

        [MaxLength(60)]
        [SpecialName()]
        public string Address { get; set; }

        [Display(Name = "Address 2")]
        [MaxLength(60)]
        [SpecialName()]
        public string Address2 { get; set; }

        [MaxLength(60)]
        [SpecialName()]
        public string City { get; set; }

        [MaxLength(60)]
        [SpecialName()]
        public string State { get; set; }

        [Display(Name = "Zip Code")]
        [MaxLength(10)]
        [SpecialName()]
        public string ZipCode { get; set; }

        [MaxLength(60)]
        [SpecialName()]
        public string Country { get; set; }
        #endregion
    }
}
