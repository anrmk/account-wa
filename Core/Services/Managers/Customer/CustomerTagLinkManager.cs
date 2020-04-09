using Core.Context;
using Core.Data.Entities;
using Core.Services.Base;

namespace Core.Services.Managers {
    public interface ICustomerTagLinkManager: IEntityManager<CustomerTagLinkEntity> {
    }

    public class CustomerTagLinkManager: AsyncEntityManager<CustomerTagLinkEntity>, ICustomerTagLinkManager {
        public CustomerTagLinkManager(IApplicationContext context) : base(context) { }
    }
}
