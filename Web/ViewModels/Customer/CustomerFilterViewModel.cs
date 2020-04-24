using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Web.ViewModels {
    public class CustomerFilterViewModel: PagerFilterViewModel {
        [Display(Name = "Company")]
        public long? CompanyId { get; set; }

        [Display(Name = "Period Date")]
        [DataType(DataType.Date)]
        [FromQuery]
        public DateTime? DateFrom { get; set; }

        [Display(Name = "Period Date")]
        [DataType(DataType.Date)]
        [FromQuery]
        public DateTime? DateTo { get; set; }

        [Display(Name = "Summary Range")]
        public long? SummaryRangeId { get; set; }

        [Display(Name = "Tags")]
        [FromQuery(Name = "TagsIds")]
        public List<long> TagsIds { get; set; }

        [Display(Name = "Type")]
        [FromQuery(Name = "TypeIds")]
        public List<long> TypeIds { get; set; }

        [Display(Name = "Recheck")]
        public List<int> Recheck { get; set; }

        [Display(Name = "Created Date")]
        [DataType(DataType.Date)]
        [FromQuery]
        public DateTime? CreatedDate { get; set; }
    }
}
