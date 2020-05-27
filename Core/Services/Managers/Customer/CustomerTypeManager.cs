using Core.Context;
using Core.Data.Entities.Nsi;
using Core.Services.Base;

namespace Core.Services.Managers {
    public interface ICustomerTypeManager: IEntityManager<CustomerTypeEntity> {

    }

    public class CustomerTypeManager: AsyncEntityManager<CustomerTypeEntity>, ICustomerTypeManager {
        public CustomerTypeManager(IApplicationContext context) : base(context) { }
    }
}
