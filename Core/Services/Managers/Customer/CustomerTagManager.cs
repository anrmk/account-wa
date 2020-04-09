using Core.Context;
using Core.Data.Entities;
using Core.Services.Base;

namespace Core.Services.Managers {
    public interface ICustomerTagManager: IEntityManager<CustomerTagEntity> {
    }

    public class CustomerTagManager: AsyncEntityManager<CustomerTagEntity>, ICustomerTagManager {
        public CustomerTagManager(IApplicationContext context) : base(context) { }
    }
}
