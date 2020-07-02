using Core.Context;
using Core.Data.Entities;
using Core.Services.Base;

namespace Core.Services.Managers {
    public interface ICustomerSettingsRestrictedWordManager: IEntityManager<CustomerSettingsRestrictedWordEntity> {

    }

    public class CustomerSettingsRestrictedWordManager: AsyncEntityManager<CustomerSettingsRestrictedWordEntity>, ICustomerSettingsRestrictedWordManager {
        public CustomerSettingsRestrictedWordManager(IApplicationContext context) : base(context) { }
    }
}
