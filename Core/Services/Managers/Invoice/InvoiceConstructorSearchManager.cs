using Core.Context;
using Core.Data.Entities;
using Core.Services.Base;

namespace Core.Services.Managers {
    public interface IInvoiceConstructorSearchManager: IEntityManager<InvoiceConstructorSearchEntity> {
    }

    public class InvoiceConstructorSearchManager: AsyncEntityManager<InvoiceConstructorSearchEntity>, IInvoiceConstructorSearchManager {
        public InvoiceConstructorSearchManager(IApplicationContext context) : base(context) { }

    }
}
