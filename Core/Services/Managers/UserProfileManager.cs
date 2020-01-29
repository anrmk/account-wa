using Core.Context;
using Core.Data.Entities;
using Core.Services.Base;

namespace Core.Services.Managers {
    public interface IUserProfileManager: IEntityManager<UserProfileEntity> {
    }

    public class UserProfileManager: AsyncEntityManager<UserProfileEntity>, IUserProfileManager {
        public UserProfileManager(IApplicationContext context) : base(context) { }
    }
}
