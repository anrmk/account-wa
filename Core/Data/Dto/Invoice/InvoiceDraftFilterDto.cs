using Core.Extension;

namespace Core.Data.Dto {
    public class InvoiceDraftFilterDto: PagerFilter {
        public long? ConstructorId { get; set; }
    }
}
