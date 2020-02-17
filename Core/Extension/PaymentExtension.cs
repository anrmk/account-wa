using System;
using System.Collections.Generic;
using System.Linq;

using Core.Data.Dto;

namespace Core.Extension {
    public static class PaymentExtension {
        public static decimal TotalAmount(this ICollection<PaymentDto> collection) {
            if(collection != null && collection.Count > 0) {
                return collection.Sum(x => x.Amount);
            }
            return 0;
        }

        public static DateTime? LastPaymentDate(this ICollection<PaymentDto> collection) {
            if(collection != null && collection.Count > 0) {
                var result = collection.OrderByDescending(x => x.Date).FirstOrDefault();
                return result?.Date;
            }
            return (DateTime?)null;
        }
    }
}
