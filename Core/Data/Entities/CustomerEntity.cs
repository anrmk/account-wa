using System;
using System.Collections.Generic;

namespace Core.Data.Entities {
    public class CustomerEntity: EntityBase<long> {
        public string AccountNumber { get; set; }
        public string BusinessName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public int Terms { get; set; }
        public DateTime DateOpen { get; set; }

        //public ICollection<Period> Periods { get; set; }


        public ICollection<InvoiceEntity> Invoices { get; set; }
    }
}
