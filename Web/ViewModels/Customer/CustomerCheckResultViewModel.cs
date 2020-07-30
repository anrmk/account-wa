using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.ViewModels {
    public class CustomerCheckResultViewModel {
        public int[] Ids { get; set; }
        public string Message { get; set; }
        public bool IsError { get; set; } = false;
    }
}
