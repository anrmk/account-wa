using Core.Data.Enum;

namespace Core.Data.Dto {
    public class CompanySettingsDto {
        public long Id { get; set; }

        public RoundType RoundType { get; set; }

        public bool SaveCreditValues { get; set; }

        public string AccountNumberTemplate { get; set; }
    }
}
