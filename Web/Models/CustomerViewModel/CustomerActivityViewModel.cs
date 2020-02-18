using System;

namespace Web.ViewModels {
    public class CustomerActivityViewModel {
        public long Id { get; set; }

        public long? CustomerId { get; set; }
        public DateTime CreatedDate { get; set; }

        public bool IsActive { get; set; }
    }
}
