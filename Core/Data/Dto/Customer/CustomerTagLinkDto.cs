namespace Core.Data.Dto {
    public class CustomerTagLinkDto {
        public long Id { get; set; }
        public long? CustomerId { get; set; }
        public long? TagId { get; set; }
    }
}
