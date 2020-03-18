using Core.Context;
using Core.Data.Entities.Nsi;
using Core.Services.Base;

namespace Core.Services.Managers.Nsi {
    public interface IRecheckManager: IEntityManager<RecheckEntity> {
    }

    public class RecheckManager: AsyncEntityManager<RecheckEntity>, IRecheckManager {
        public RecheckManager(IApplicationContext context) : base(context) { }
    }
}
