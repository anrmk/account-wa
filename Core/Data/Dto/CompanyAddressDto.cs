namespace Core.Data.Dto {
    public class CompanyAddressDto {
        public long? Id { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }

        public string FullAddress => $"{ Address}, {City}, {State}, {ZipCode}, {Country}".Trim(',').Trim();
    }
}
