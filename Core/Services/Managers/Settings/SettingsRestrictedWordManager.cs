using Core.Context;
using Core.Data.Entities;
using Core.Services.Base;

namespace Core.Services.Managers {
    public interface ISettingsRestrictedWordManager: IEntityManager<SettingsRestrictedWordEntity> {

    }

    public class SettingsRestrictedWordManager: AsyncEntityManager<SettingsRestrictedWordEntity>, ISettingsRestrictedWordManager {
        public SettingsRestrictedWordManager(IApplicationContext context) : base(context) { }
    }
}
