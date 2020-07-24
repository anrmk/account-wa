namespace Web.ViewModels {
    public class SavedReportPlanFieldViewModel {
        public long Id { get; set; }

        public string Name { get; set; }

        public int Count { get; set; }

        public decimal? Amount { get; set; }

        //public long? ReportId { get; set; }

        public bool AmountIsRequired { get; set; }
        public bool CountIsRequired { get; set; }

        public bool AmountDisplay { get; set; } = true;
        public bool CountDisplay { get; set; } = true;

        public bool AmountReadOnly { get; set; } = false;
        public bool CountReadOnly { get; set; } = false;
    }
}
