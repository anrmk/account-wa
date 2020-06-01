using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.ViewModels {
    public class InvoiceDraftFilterViewModel: PagerFilterViewModel {
        public long? ConstructorId { get; set; }
    }
}
