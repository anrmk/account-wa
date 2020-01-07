namespace Core.Data.Entities {
    public class Period: EntityBase<long> {
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
