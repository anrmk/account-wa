using Core.Context;
using Core.Data.Entities;
using Core.Services.Base;

namespace Core.Services.Managers {

    public interface IInvoiceConstructorManager: IEntityManager<InvoiceConstructorEntity> {

    }

    public class InvoiceConstructorManager: AsyncEntityManager<InvoiceConstructorEntity>, IInvoiceConstructorManager {
        public InvoiceConstructorManager(IApplicationContext context) : base(context) { }
    }
}
