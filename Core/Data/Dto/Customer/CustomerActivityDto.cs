using System;

namespace Core.Data.Dto {
    public class CustomerActivityDto {
        public long Id { get; set; }

        public long? CustomerId { get; set; }
        public virtual CustomerDto Customer { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsActive { get; set; }
    }
}
