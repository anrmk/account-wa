using System;

using Core.Data.Enum;

namespace Core.Data.Dto {
    public class CustomerCreditUtilizedSettingsDto {
        public long Id { get; set; }

        public long? CompanyId { get; set; }

        public RoundType RoundType { get; set; }

        public DateTime Date { get; set; }
    }
}
