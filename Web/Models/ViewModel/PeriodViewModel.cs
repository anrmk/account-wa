using System;

namespace Web.ViewModels {
    public class PeriodViewModel {
        public long Id { get; set; }
        public DateTime Value { get; set; }

        public string Display => Value.ToString("MMMM yyyy");
    }
}
