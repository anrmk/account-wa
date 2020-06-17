using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Core.Data.Enum;

namespace Web.ViewModels {
    public class CustomerCreditUtilizedSettingsViewModel {
        public long Id { get; set; }

        [Required]
        public long CompanyId { get; set; }

        [Required]
        public RoundType RoundType { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        public int NumberOfPeriods { get; set; }
    }
}
