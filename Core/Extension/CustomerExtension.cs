using System;
using System.Collections.Generic;
using System.Linq;

using Core.Data.Entities;

namespace Core.Extension {
    public static class CustomerExtension {
        public static bool IsActive(this ICollection<CustomerActivityEntity> collection) {
            if(collection != null && collection.Count > 0) {
                var lastRecord = collection
                    .OrderByDescending(x => x.CreatedDate)
                    .Select(x => x.IsActive).FirstOrDefault();
                return lastRecord || false;
            }
            return false;
        }

        public static bool IsActive(this ICollection<CustomerActivityEntity> collection, DateTime dateTo) {
            if(collection != null && collection.Count > 0) {
                var lastRecord = collection.OrderByDescending(x => x.CreatedDate);
                var where = lastRecord.Where(x => x.CreatedDate <= dateTo);
                var select = where.Select(x => x.IsActive).FirstOrDefault();

                return select || false;
            }
            return false;
        }
    }
}
