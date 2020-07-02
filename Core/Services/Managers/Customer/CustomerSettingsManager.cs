
using Core.Context;
using Core.Data.Entities;
using Core.Services.Base;

namespace Core.Services.Managers {
    public interface ICustomerSettingsManager: IEntityManager<CustomerSettingsEntity> {
    }

    public class CustomerSettingsManager: AsyncEntityManager<CustomerSettingsEntity>, ICustomerSettingsManager {
        public CustomerSettingsManager(IApplicationContext context) : base(context) { }
    }
}
