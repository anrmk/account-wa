namespace Core.Data.Dto {
    public class PagerFilterDto {
        public string Search { get; set; }
        public string Sort { get; set; }
        public string Order { get; set; }
        public int Offset { get; set; }
        public int Limit { get; set; }
        public bool RandomSort { get; set; } = false;
    }
}
